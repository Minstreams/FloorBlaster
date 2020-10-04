using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using GameSystem.Networking.Packet;

namespace GameSystem
{
    namespace Networking
    {
        /// <summary>
        /// Server of NetworkSystem
        /// </summary>
        public class Server
        {
            #region 流程相关 -------------------------------------
            public enum ServerState
            {
                room,
                pendingStart,
                inGame,
            }
            public ServerState currentState = ServerState.room;
            private void UDPReceive(UDPPacket packet)
            {
                if (currentState != ServerState.room) return;
                if (packet.message != NetworkSystem.clientHello) return;
                UDPSend(NetworkSystem.PacketToString(new PacketRoomInfo(NetworkSystem.serverHi)), packet.endPoint);
            }

            private void TCPReceive(TCPPacket packet)
            {
                NetworkSystem.CallProcessPacket(packet.message);
            }

            #endregion

            #region API ------------------------------------------
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
            #endregion

            #region Inner Code -----------------------------------
            private Setting.NetworkSystemSetting Setting { get { return NetworkSystem.Setting; } }
            private List<Connection> connections = new List<Connection>();
            private bool isDestroyed = false;
            private static void Log(object msg)
            {
                if (!TheMatrix.debug) return;
                string msgStr = "[Server]" + msg.ToString();
                NetworkSystem.CallLog(msgStr);
            }
            private static void Log(SocketException ex)
            {
                NetworkSystem.CallLog("[Server Exception]" + ex.GetType().Name + "|" + ex.SocketErrorCode + ":" + ex.Message + "\n" + ex.StackTrace);
            }
            private static void Log(Exception ex)
            {
                NetworkSystem.CallLog("[Server Exception]" + ex.GetType().Name + ":" + ex.Message + "\n" + ex.StackTrace);
            }

            public Server()
            {
                TheMatrix.StartCoroutine(ConnectionThread(), typeof(Server));
                listener = new TcpListener(new IPEndPoint(IPAddress.Parse(NetworkSystem.LocalIP), Setting.serverTCPPort));
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
            #endregion

            #region UDP-------------------------------------------
            private UdpClient udpClient;
            private Thread udpReceiveThread;

            public event Action<UDPPacket> onUDPReceive;
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
                        UDPPacket packet = new UDPPacket(receiveString, remoteIP);
                        onUDPReceive?.Invoke(packet);
                        UDPReceive(packet);
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
            private TcpListener listener;
            private Thread listenThread;
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
                    Log(ex);
                    NetworkSystem.ShutdownServer();
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
            #endregion

            /// <summary>
            /// 在服务器上运行的玩家连接
            /// </summary>
            public class Connection
            {
                #region API ------------------------------------------
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
                    receiveThread?.Abort();
                    stream?.Close();
                    client?.Close();
                }
                public void Send(string message)
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    stream.Write(messageBytes, 0, messageBytes.Length);
                }
                #endregion

                #region Inner Code -----------------------------------
                private bool isDestroyed = false;
                private Server server;
                private TcpClient client;
                private NetworkStream stream;
                private Thread receiveThread;
                private byte[] buffer = new byte[NetworkSystem.maxMsgLength];

                public event Action<string> onReceive;

                public Connection(TcpClient client, Server server)
                {
                    this.server = server;
                    this.client = client;
                    stream = client.GetStream();
                    receiveThread = new Thread(ReceiveThread);
                    receiveThread.Start();

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
                            server.TCPReceive(new TCPPacket(receiveString, this));
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
}