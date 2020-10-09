using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

namespace GameSystem.Networking
{
    /// <summary>
    /// Server of NetworkSystem
    /// </summary>
    public class Server
    {
        #region 流程相关 -------------------------------------
        // 这里可以写临时功能，但是最好不要在这层实现流程控制

        #endregion

        #region API ------------------------------------------
        public bool TcpOn { get; private set; } = false;
        public void Destroy()
        {
            if (isDestroyed)
            {
                Log("Disposed.");
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
            NetworkSystem.CallMainThread(() =>
            {
                conn.Destroy();
                connections.Remove(conn);
            });
        }
        public void UDPSend(string message, IPEndPoint remote)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            udpClient.Send(messageBytes, messageBytes.Length, remote);
            Log($"UDPSend{remote}:{message}");
        }
        public void UDPBoardcast(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            udpClient.Send(messageBytes, messageBytes.Length, new IPEndPoint(IPAddress.Broadcast, Setting.clientUDPPort));
            Log($"UDPBoardcast:{message}");
        }

        public void TurnOnTCP()
        {
            TcpOn = true;
            listener = new TcpListener(new IPEndPoint(NetworkSystem.LocalIPAddress, Setting.serverTCPPort));
            listenThread = new Thread(ListenThread);
            listenThread.Start();
        }

        #endregion

        #region Inner Code -----------------------------------
        Setting.NetworkSystemSetting Setting { get { return NetworkSystem.Setting; } }
        List<Connection> connections = new List<Connection>();
        bool isDestroyed = false;
        static void Log(object msg)
        {
            if (!TheMatrix.debug) return;
            string msgStr = "[Server]" + msg.ToString();
            NetworkSystem.CallLog(msgStr);
        }
        static void Log(SocketException ex)
        {
            NetworkSystem.CallLog("[Server Exception]" + ex.GetType().Name + "|" + ex.SocketErrorCode + ":" + ex.Message + "\n" + ex.StackTrace);
        }
        static void Log(Exception ex)
        {
            NetworkSystem.CallLog("[Server Exception]" + ex.GetType().Name + ":" + ex.Message + "\n" + ex.StackTrace);
        }

        public Server()
        {
            udpClient = new UdpClient(Setting.serverUDPPort);
            udpReceiveThread = new Thread(UDPReceiveThread);
            udpReceiveThread.Start();

            Log("服务端已启用……|UDP:" + Setting.serverUDPPort);
        }
        ~Server()
        {
            Log("~Server");
            Destroy();
        }
        #endregion

        #region UDP-------------------------------------------
        UdpClient udpClient;
        Thread udpReceiveThread;

        void UDPReceiveThread()
        {
            Log("开始收UDP包……");
            while (true)
            {
                try
                {
                    IPEndPoint remoteIP = new IPEndPoint(IPAddress.Any, Setting.clientUDPPort);
                    byte[] buffer = udpClient.Receive(ref remoteIP);
                    string receiveString = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                    Log($"UDPReceive{remoteIP}:{receiveString}");
                    UDPPacket packet = new UDPPacket(receiveString, remoteIP);
                    NetworkSystem.CallProcessUDPPacket(packet);
                }
                catch (SocketException ex)
                {
                    Log(ex);
                    continue;
                }
                catch (ThreadAbortException)
                {
                    Log("UDPReceive Thread Aborted.");
                    return;
                }
                catch (Exception ex)
                {
                    Log(ex);
                    NetworkSystem.ShutdownServer();
                    return;
                }
            }
        }
        #endregion

        #region TCP ------------------------------------------
        TcpListener listener;
        Thread listenThread;
        void ListenThread()
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
                Log(ex);
                NetworkSystem.ShutdownServer();
            }
        }
        string NewTcpId(IPEndPoint ip)
        {
            if (ip.Address.Equals(NetworkSystem.LocalIPAddress) && ip.Port.Equals(NetworkSystem.LocalIPPort)) return "0";
            int output = 1;
            foreach (Connection conn in connections)
            {
                int id = int.Parse(conn.netId);
                if (output <= id) output = id + 1;
            }
            return output.ToString();
        }
        void CallConnect(TcpClient client)
        {
            NetworkSystem.CallMainThread(() =>
            {
                connections.Add(new Connection(client, this, NewTcpId(client.Client.RemoteEndPoint as IPEndPoint)));
            });
        }
        #endregion

        /// <summary>
        /// 在服务器上运行的玩家连接
        /// </summary>
        public class Connection
        {
            #region API ------------------------------------------
            public IPEndPoint RemoteEndPoint { get { return (IPEndPoint)client.Client.RemoteEndPoint; } }
            public IPEndPoint UDPEndPoint { get { return new IPEndPoint(RemoteEndPoint.Address, NetworkSystem.Setting.clientUDPPort); } }
            public string netId;
            public bool isHost;
            public void Destroy()
            {
                if (isDestroyed)
                {
                    Log("Disposed.");
                    return;
                }
                isDestroyed = true;

                Log("Destroy");
                receiveThread?.Abort();
                NetworkSystem.CallProcessDisconnection(this);
                stream?.Close();
                client?.Close();
            }
            public void Send(string message)
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message + NetworkSystem.overMark);
                stream.Write(messageBytes, 0, messageBytes.Length);
            }
            public void Send(PacketBase packet)
            {
                Send(NetworkSystem.PacketToString(packet));
            }
            #endregion

            #region Inner Code -----------------------------------
            bool isDestroyed = false;
            Server server;
            TcpClient client;
            NetworkStream stream;
            Thread receiveThread;
            byte[] buffer = new byte[NetworkSystem.maxMsgLength];
            string receiveStringBuffer = "";
            Regex PacketCutter = new Regex(@"^([^✡]*)✡(.*)$");


            public Connection(TcpClient client, Server server, string netId)
            {
                this.server = server;
                this.client = client;
                this.netId = netId;
                this.isHost = netId.Equals("0");
                stream = client.GetStream();
                receiveThread = new Thread(ReceiveThread);
                receiveThread.Start();
                Send(netId);
                NetworkSystem.CallProcessConnection(this);

                Log("已连接。");
            }
            ~Connection()
            {
                Log("~Connection");
                Destroy();
            }
            void ReceiveThread()
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
                        receiveStringBuffer += receiveString;

                        Match match = PacketCutter.Match(receiveStringBuffer);
                        while (match.Success)
                        {
                            NetworkSystem.CallProcessPacket(match.Groups[1].Value, this);
                            receiveStringBuffer = match.Groups[2].Value;
                            match = PacketCutter.Match(receiveStringBuffer);
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    Log("Receive Thread Aborted.");
                }
                catch (Exception ex)
                {
                    Log(ex);
                    server.CloseConnection(this);
                }
            }
            #endregion
        }
    }
}
