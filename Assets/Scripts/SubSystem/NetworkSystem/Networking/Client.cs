using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace GameSystem
{
    namespace Networking
    {
        /// <summary>
        /// Client and Connection of NetworkSystem
        /// </summary>
        public class Client
        {
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

                StopUDPBoardcast();
                StopTCPConnecting();
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
                if (udpClient == null)
                {
                    Log("UDP not opened!");
                    return;
                }
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                udpClient.Send(messageBytes, messageBytes.Length, remote);
            }
            public void UDPSend(string ip, string message)
            {
                if (udpClient == null)
                {
                    Log("UDP not opened!");
                    return;
                }
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                try
                {
                    udpClient.Send(messageBytes, messageBytes.Length, new IPEndPoint(IPAddress.Parse(ip), Setting.serverUDPPort));
                }
                catch (Exception ex)
                {
                    Log(ex);
                }
            }

            public void OpenUDP()
            {
                if (udpClient != null) return;
                do
                {
                    try
                    {
                        udpClient = new UdpClient(port - 1);
                    }
                    catch (SocketException ex)
                    {
                        Log(ex);
                        if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                        {
                            port = NetworkSystem.GetValidPort();
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(ex);
                        CloseUDP();
                        return;
                    }
                } while (false);
            }
            public void CloseUDP()
            {
                udpClient?.Close();
                udpClient = null;
            }

            /// <summary>
            /// 开始广播寻找服务器
            /// </summary>
            public void StartUDPBoardcast()
            {
                try
                {
                    OpenUDP();
                    udpReceiveThread = new Thread(UDPReceiveThread);
                    udpReceiveThread.Start();
                    udpBoardcastThread = new Thread(UDPBoardcastThread);
                    udpBoardcastThread.Start();
                }
                catch (Exception ex)
                {
                    Log(ex);
                    StopUDPBoardcast();
                    return;
                }
            }
            /// <summary>
            /// 结束广播，停止占用端口
            /// </summary>
            public void StopUDPBoardcast()
            {
                udpBoardcastThread?.Abort();
                udpReceiveThread?.Abort();
                CloseUDP();
            }
            /// <summary>
            /// 开始tcp连接，加入房间
            /// </summary>
            public void StartTCPConnecting()
            {
                do
                {
                    try
                    {
                        client = new TcpClient(new IPEndPoint(IPAddress.Parse(NetworkSystem.LocalIP), port));
                        connectThread = new Thread(ConnectThread);
                        connectThread.Start();
                    }
                    catch (SocketException ex)
                    {
                        Log(ex);
                        StopUDPBoardcast();
                        if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse) port = NetworkSystem.GetValidPort();
                        continue;
                    }
                    catch (Exception ex)
                    {
                        Log(ex);
                        StopTCPConnecting();
                        return;
                    }
                } while (false);
            }
            /// <summary>
            /// 结束tcp连接
            /// </summary>
            public void StopTCPConnecting()
            {
                connectThread?.Abort();
                receiveThread?.Abort();
                stream?.Close();
                client?.Close();
            }
            #endregion

            #region 流程相关 -------------------------------------
            private Thread udpBoardcastThread;
            private void UDPBoardcastThread()
            {
                // 向所有的PossibleIP发送Hello
                string[] ips = NetworkSystem.GetPossibleIPs();
                try
                {
                    while (true)
                    {
                        for (int i = 0; i < ips.Length; i++)
                        {
                            UDPSend(ips[i], NetworkSystem.clientHello);
                            Thread.Sleep(Setting.udpBoardcastInterval);
                        }
                        Thread.Sleep(Setting.udpBoardcastRefreshInterval);
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Log(ex);
                    StopUDPBoardcast();
                    return;
                }
            }

            private void TCPReceive(string packetMessage)
            {
                NetworkSystem.CallReceive(packetMessage);
            }
            #endregion

            #region Inner Code -----------------------------------
            private Setting.NetworkSystemSetting Setting { get { return NetworkSystem.Setting; } }
            private int port;
            private bool isDestroyed = false;

            public Client()
            {
                try
                {
                    port = NetworkSystem.GetValidPort();
                    Log("客户端已启用……");
                }
                catch (Exception ex)
                {
                    Log(ex);
                    NetworkSystem.ShutdownClient();
                    return;
                }
            }
            ~Client()
            {
                Log("~Client");
                Destroy();
            }
            #endregion

            #region UDP-------------------------------------------
            private UdpClient udpClient;
            private Thread udpReceiveThread;

            private void UDPReceiveThread()
            {
                Log("开始收UDP包……");
                while (true)
                {
                    try
                    {
                        IPEndPoint remoteIP = new IPEndPoint(IPAddress.Any, port - 1);
                        byte[] buffer = udpClient.Receive(ref remoteIP);
                        string receiveString = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        Log($"UDPReceive{remoteIP}:{receiveString}");
                        NetworkSystem.CallUDPReceive(new UDPPacket(receiveString, remoteIP));
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
                        NetworkSystem.ShutdownClient();
                        return;
                    }
                }
            }
            #endregion

            #region TCP ------------------------------------------

            private TcpClient client;
            private Thread connectThread;
            private NetworkStream stream;
            private Thread receiveThread;
            private byte[] buffer = new byte[NetworkSystem.maxMsgLength];

            private void ConnectThread()
            {
                do
                {
                    Log("Connecting……");
                    try
                    {
                        client.Connect(NetworkSystem.ServerIPEndPoint);
                        // Block --------------------------------
                    }
                    catch (SocketException ex)
                    {
                        Log(ex);
                        Log("连接失败！重新连接中……");
                        if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse) port = NetworkSystem.GetValidPort();
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
                        Log(ex);
                        NetworkSystem.ShutdownClient();
                        return;
                    }
                } while (!client.Connected);

                Log("已连接……");
                stream = client.GetStream();
                receiveThread = new Thread(ReceiveThread);
                receiveThread.Start();
                NetworkSystem.CallConnected();
            }
            private void ReceiveThread()
            {
                string receiveString;

                int count;
                count = stream.Read(buffer, 0, buffer.Length);
                // Block --------------------------------
                if (count <= 0)
                {
                    Log("与服务器断开连接");
                    NetworkSystem.ShutdownClient();
                    NetworkSystem.CallDisconnected();
                    return;
                }
                receiveString = Encoding.UTF8.GetString(buffer, 0, count);
                NetworkSystem.netId = receiveString;

                try
                {
                    while (true)
                    {
                        count = stream.Read(buffer, 0, buffer.Length);
                        // Block --------------------------------
                        if (count <= 0)
                        {
                            Log("与服务器断开连接");
                            NetworkSystem.ShutdownClient();
                            NetworkSystem.CallDisconnected();
                            return;
                        }
                        receiveString = Encoding.UTF8.GetString(buffer, 0, count);
                        Log($"Receive{client.Client.LocalEndPoint}:{receiveString}");
                        TCPReceive(receiveString);
                    }
                }
                catch (ThreadAbortException)
                {
                    Log("Receive Thread Aborted.");
                }
                catch (Exception ex)
                {
                    Log(ex);
                    NetworkSystem.ShutdownClient();
                }
            }
            #endregion

            private static void Log(object msg)
            {
                if (!TheMatrix.debug) return;
                string msgStr = "[Client]" + msg.ToString();
                NetworkSystem.CallLog(msgStr);
            }
            private static void Log(SocketException ex)
            {
                NetworkSystem.CallLog("[Client Exception]" + ex.GetType().Name + "|" + ex.SocketErrorCode + ":" + ex.Message + "\n" + ex.StackTrace);
            }
            private static void Log(Exception ex)
            {
                NetworkSystem.CallLog("[Client Exception]" + ex.GetType().Name + ":" + ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
