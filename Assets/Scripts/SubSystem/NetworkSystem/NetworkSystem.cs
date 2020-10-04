using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Setting;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Net.NetworkInformation;

namespace GameSystem
{
    /// <summary>
    /// 网络系统。用于多人联机和服务器服务。
    /// </summary>
    public class NetworkSystem : SubSystem<NetworkSystemSetting>
    {
        private const int maxMsgLength = 2048;


        private static string ServerIP { get { return Setting.serverIP; } }
        private static string LocalIP { get { return Setting.localIP; } }

        public static bool isServer { get; private set; }

        /// <summary>
        /// Server of NetworkSystem
        /// </summary>
        public class Server
        {
            // API ------------------------------------------
            public void Destroy()
            {
                if (isDestroyed)
                {
                    Log("Destroy Again.");
                    return;
                }
                isDestroyed = true;
                Log("Destroy");
                TheMatrix.StopAllCoroutines(typeof(Server));

                udpReceiveThread?.Abort();
                udpClient.Close();

                listenThread?.Abort();
                connections?.ForEach(conn => { conn.Destroy(); });
                connections?.Clear();
                listener?.Stop();
            }
            public void Boardcast(string message)
            {
                connections.ForEach(conn => { conn.Send(message); });
            }
            public void CloseConnection(Connection conn)
            {
                pendingCloseQueue.Enqueue(conn);
            }
            public void UDPSend(string message, IPEndPoint remote)
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                udpClient.Send(messageBytes, messageBytes.Length, remote);
            }
            public void UDPBoardcast(string message)
            {
                connections.ForEach(conn => { UDPSend(message, conn.RemoteEndPoint); });
            }

            // Inner Code -----------------------------------
            private bool isDestroyed = false;

            public Server()
            {
                TheMatrix.StartCoroutine(ConnectionThread(), typeof(Server));
                listener = new TcpListener(new IPEndPoint(IPAddress.Parse(ServerIP), Setting.serverTCPPort));
                listenThread = new Thread(ListenThread);
                listenThread.Start();

                udpClient = new UdpClient(Setting.serverUDPPort);
                udpReceiveThread = new Thread(UDPReceiveThread);
                udpReceiveThread.Start();

                Log("服务端已启用……");
            }
            ~Server()
            {
                Log("~Server");
                Destroy();
            }

            // UDP-------------------------------------------
            private UdpClient udpClient;
            private Thread udpReceiveThread;

            public event Action<string> onUDPReceive;
            private void UDPReceiveThread()
            {
                Log("开始收UDP包……");
                while (true)
                {
                    try
                    {
                        IPEndPoint remoteIP = new IPEndPoint(IPAddress.Any, Setting.serverUDPPort);
                        byte[] buffer = udpClient.Receive(ref remoteIP);
                        string receiveString = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        Log($"UDPReceive{remoteIP}:{receiveString}");
                        onUDPReceive?.Invoke(receiveString);
                    }
                    catch (SocketException ex)
                    {
                        Log(ex.Message + "\n" + ex.StackTrace);
                        continue;
                    }
                    catch (ThreadAbortException)
                    {
                        Log("UDPReceive Thread Aborted.");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Log(ex.Message + "\n" + ex.StackTrace);
                        ShutdownServer();
                        return;
                    }
                }
            }



            // TCP ------------------------------------------
            private TcpListener listener;
            private Thread listenThread;
            private List<Connection> connections = new List<Connection>();
            private void ListenThread()
            {
                try
                {
                    listener.Start();
                    while (true)
                    {
                        Log("Listening……");
                        CallConnect(listener.AcceptTcpClient());
                        // Block --------------------------------
                    }
                }
                catch (ThreadAbortException)
                {
                    Log("Listen Thread Aborted");
                }
                catch (Exception ex)
                {
                    Log(ex.Message + "\n" + ex.StackTrace);
                    ShutdownServer();
                }
            }
            private Queue<TcpClient> pendingConnectionQueue = new Queue<TcpClient>();
            private Queue<Connection> pendingCloseQueue = new Queue<Connection>();
            private void CallConnect(TcpClient client)
            {
                pendingConnectionQueue.Enqueue(client);
            }
            private IEnumerator ConnectionThread()
            {
                while (true)
                {
                    yield return 0;
                    while (pendingConnectionQueue.Count > 0) connections.Add(new Connection(pendingConnectionQueue.Dequeue(), this));
                    while (pendingCloseQueue.Count > 0)
                    {
                        var conn = pendingCloseQueue.Dequeue();
                        conn.Destroy();
                        connections.Remove(conn);
                    }
                }
            }


            private static void Log(object msg)
            {
                string msgStr = "[Server]" + msg.ToString();
                Console.WriteLine(msgStr);
                CallLog(msgStr);
            }
            /// <summary>
            /// 在服务器上运行的玩家连接
            /// </summary>
            public class Connection
            {
                // API ------------------------------------------
                public IPEndPoint RemoteEndPoint { get { return (IPEndPoint)client.Client.RemoteEndPoint; } }
                public IPEndPoint UDPEndPoint { get { return new IPEndPoint(RemoteEndPoint.Address, RemoteEndPoint.Port - 1); } }
                public void Destroy()
                {
                    if (isDestroyed)
                    {
                        Log("Destroy Again.");
                        return;
                    }
                    isDestroyed = true;

                    Log("Destroy");
                    TheMatrix.StopCoroutine(receiveEventCoroutine);
                    MonoBehaviour.Destroy(agent);
                    receiveThread?.Abort();
                    stream?.Close();
                    client?.Close();
                }
                public void Send(string message)
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    stream.Write(messageBytes, 0, messageBytes.Length);
                }


                // Inner Code -----------------------------------
                private bool isDestroyed = false;
                private Server server;
                private TcpClient client;
                private NetworkStream stream;
                private Operator.ServerConnectionAgent agent;
                private Thread receiveThread;
                private byte[] buffer = new byte[maxMsgLength];

                public event Action<string> onReceive;

                public Connection(TcpClient client, Server server)
                {
                    receiveEventCoroutine = TheMatrix.StartCoroutine(ReceiveEventThread(), typeof(Connection));
                    this.server = server;
                    this.client = client;
                    stream = client.GetStream();
                    receiveThread = new Thread(ReceiveThread);
                    receiveThread.Start();
                    agent = TheMatrix.Instance.gameObject.AddComponent<Operator.ServerConnectionAgent>();
                    agent.Init(this);

                    Log("已连接。");
                }
                ~Connection()
                {
                    Log("~Connection");
                    Destroy();
                }
                private void ReceiveThread()
                {
                    string receiveString;

                    int count;
                    try
                    {
                        while (true)
                        {
                            count = stream.Read(buffer, 0, buffer.Length);
                            // Block --------------------------------
                            if (count <= 0)
                            {
                                Log("与客户端断开连接");
                                server.CloseConnection(this);
                                return;
                            }
                            receiveString = Encoding.UTF8.GetString(buffer, 0, count);
                            Log($"Receive{client.Client.LocalEndPoint}:{receiveString}");
                            CallReceiveEvent(receiveString);
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        Log("Receive Thread Aborted.");
                    }
                    catch (Exception ex)
                    {
                        Log(ex.Message + "\n" + ex.StackTrace);
                        server.CloseConnection(this);
                    }
                }

                private LinkedListNode<Coroutine> receiveEventCoroutine;
                private Queue<string> pendingReceiveQueue = new Queue<string>();
                private void CallReceiveEvent(string message)
                {
                    pendingReceiveQueue.Enqueue(message);
                }
                private IEnumerator ReceiveEventThread()
                {
                    while (true)
                    {
                        yield return 0;
                        while (pendingReceiveQueue.Count > 0) onReceive?.Invoke(pendingReceiveQueue.Dequeue());
                    }
                }
            }
        }
        /// <summary>
        /// Client and Connection of NetworkSystem
        /// </summary>
        public class Client
        {
            // API ------------------------------------------
            public void Destroy()
            {
                if (isDestroyed)
                {
                    Log("Destroy Again.");
                    return;
                }
                isDestroyed = true;

                TheMatrix.StopAllCoroutines(typeof(Client));
                Log("Destroy");

                udpReceiveThread?.Abort();
                udpClient.Close();

                connectThread?.Abort();
                receiveThread?.Abort();
                MonoBehaviour.Destroy(agent);
                stream?.Close();
                client?.Close();
            }
            public void Send(string message)
            {
                if (stream == null || !stream.CanWrite)
                {
                    Log("Sending failed.");
                }
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                stream.Write(messageBytes, 0, messageBytes.Length);
            }
            public void UDPSend(string message, IPEndPoint remote)
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                udpClient.Send(messageBytes, messageBytes.Length, remote);
            }
            public void UDPSend(string message)
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                udpClient.Send(messageBytes, messageBytes.Length, new IPEndPoint(IPAddress.Parse(ServerIP), Setting.serverUDPPort));
            }


            // Inner Code -----------------------------------
            private bool isDestroyed = false;

            public Client()
            {
                TheMatrix.StartCoroutine(ReceiveEventThread(), typeof(Client));
                try
                {
                    client = new TcpClient(new IPEndPoint(IPAddress.Parse(LocalIP), port));
                    Log("客户端已启用……");
                    connectThread = new Thread(ConnectThread);
                    connectThread.Start();
                    agent = TheMatrix.Instance.gameObject.AddComponent<Operator.ClientConnectionAgent>();
                    agent.Init(this);

                    udpClient = new UdpClient(port - 1);
                    udpReceiveThread = new Thread(UDPReceiveThread);
                    udpReceiveThread.Start();
                }
                catch (Exception ex)
                {
                    Log(ex.Message + "\n" + ex.StackTrace);
                    ShutdownClient();
                    return;
                }
            }
            ~Client()
            {
                Log("~Client");
                Destroy();
            }

            // UDP-------------------------------------------
            private UdpClient udpClient;
            private Thread udpReceiveThread;

            public event Action<string> onUDPReceive;
            private void UDPReceiveThread()
            {
                Log("开始收UDP包……");
                try
                {
                    while (true)
                    {
                        IPEndPoint remoteIP = new IPEndPoint(IPAddress.Any, port - 1);
                        byte[] buffer = udpClient.Receive(ref remoteIP);
                        string receiveString = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        Log($"UDPReceive{remoteIP}:{receiveString}");
                        onUDPReceive?.Invoke(receiveString);
                    }
                }
                catch (ThreadAbortException)
                {
                    Log("UDPReceive Thread Aborted.");
                }
                catch (Exception ex)
                {
                    Log(ex.Message + "\n" + ex.StackTrace);
                    ShutdownClient();
                }
            }


            // TCP ------------------------------------------
            private const int port = 12860;

            private TcpClient client;
            private Thread connectThread;
            private NetworkStream stream;
            private Operator.ClientConnectionAgent agent;
            private Thread receiveThread;
            private byte[] buffer = new byte[maxMsgLength];

            public event Action<string> onReceive;
            private void ConnectThread()
            {
                do
                {
                    Log("Connecting……");
                    try
                    {
                        client.Connect(new IPEndPoint(IPAddress.Parse(ServerIP), Setting.serverTCPPort));
                        // Block --------------------------------
                    }
                    catch (SocketException ex)
                    {
                        Log(ex.Message + "\n" + ex.StackTrace);
                        Log("连接失败！重新连接中……");
                        Thread.Sleep(1000);
                        continue;
                    }
                    catch (ThreadAbortException)
                    {
                        Log("Connect Thread Aborted.");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Log(ex.Message + "\n" + ex.StackTrace);
                        ShutdownClient();
                        return;
                    }
                } while (!client.Connected);

                Log("已连接……");
                stream = client.GetStream();
                receiveThread = new Thread(ReceiveThread);
                receiveThread.Start();
            }

            private void ReceiveThread()
            {
                string receiveString;

                int count;
                try
                {
                    while (true)
                    {
                        count = stream.Read(buffer, 0, buffer.Length);
                        // Block --------------------------------
                        if (count <= 0)
                        {
                            Log("与服务器断开连接");
                            ShutdownClient();
                            return;
                        }
                        receiveString = Encoding.UTF8.GetString(buffer, 0, count);
                        Log($"Receive{client.Client.LocalEndPoint}:{receiveString}");
                        CallReceiveEvent(receiveString);
                    }
                }
                catch (ThreadAbortException)
                {
                    Log("Receive Thread Aborted.");
                }
                catch (Exception ex)
                {
                    Log(ex.Message + "\n" + ex.StackTrace);
                    ShutdownClient();
                }
            }

            private Queue<string> pendingReceiveQueue = new Queue<string>();
            private void CallReceiveEvent(string message)
            {
                pendingReceiveQueue.Enqueue(message);
            }
            private IEnumerator ReceiveEventThread()
            {
                while (true)
                {
                    yield return 0;
                    while (pendingReceiveQueue.Count > 0) onReceive?.Invoke(pendingReceiveQueue.Dequeue());
                }
            }

            private static void Log(object msg)
            {
                string msgStr = "[Client]" + msg.ToString();
                Console.WriteLine(msgStr);
                CallLog(msgStr);
            }

        }

        public static Server server = null;
        public static Client client = null;

        public void PingTest()
        {
            UnityEngine.Ping pinger = new UnityEngine.Ping("127.0.0.1");
        }

        public static void LaunchServer()
        {
            isServer = true;
            if (server != null)
            {
                Dialog("服务器已经存在！");
                return;
            }
            Log("Launch Server");
            server = new Server();
        }
        public static void LaunchClient()
        {
            if (client != null)
            {
                Dialog("客户端已经存在！");
                return;
            }
            Log("Launch Client");
            client = new Client();
        }
        public static void ShutdownServer()
        {
            isServer = false;
            pendingShutdownServer = true;
        }
        public static void ShutdownClient()
        {
            pendingShutdownClient = true;
        }


        /// <summary>
        /// Call Debug.Log in Main Thread
        /// </summary>
        /// <param name="message"></param>
        public static void CallLog(string message)
        {
            pendingLogQueue.Enqueue(message);
        }
        private static Queue<string> pendingLogQueue = new Queue<string>();
        private static bool pendingShutdownServer = false;
        private static bool pendingShutdownClient = false;
        private static IEnumerator MainThread()
        {
            while (true)
            {
                yield return 0;
                while (pendingLogQueue.Count > 0) Debug.Log(pendingLogQueue.Dequeue());
                if (pendingShutdownServer)
                {
                    pendingShutdownServer = false;
                    Log("Shutdown Server");
                    server?.Destroy();
                    server = null;
                }
                if (pendingShutdownClient)
                {
                    pendingShutdownClient = false;
                    Log("Shutdown Client");
                    client?.Destroy();
                    client = null;
                }
            }
        }


        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInit()
        {
            isServer = false;
            //用于控制Action初始化
            TheMatrix.onGameAwake += OnGameAwake;
            TheMatrix.onGameStart += OnGameStart;
        }
        private static void OnGameAwake()
        {
            StartCoroutine(MainThread());
            //在进入游戏第一个场景时调用
        }
        private static void OnGameStart()
        {
            //在主场景游戏开始时和游戏重新开始时调用
        }


        //API---------------------------------
        //public static void SomeFunction(){}
    }
}
