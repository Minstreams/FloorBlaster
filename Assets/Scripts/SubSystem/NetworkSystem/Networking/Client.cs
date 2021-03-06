﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

namespace GameSystem.Networking
{
    /// <summary>
    /// Client and Connection of NetworkSystem
    /// </summary>
    public class Client
    {
        #region 流程相关 -------------------------------------
        // 这里可以写临时功能，但是最好不要在这层实现流程控制

        #endregion

        #region API ------------------------------------------
        public bool IsConnected => client != null && client.Connected;
        public void Destroy()
        {
            if (isDestroyed)
            {
                Log("Disposed.");
                return;
            }
            isDestroyed = true;

            Log("Destroy");
            CloseUDP();
            StopTCPConnecting();
        }
        public void Send(string message)
        {
            if (stream == null || !stream.CanWrite)
            {
                Log("Sending failed.");
                return;
            }
            byte[] messageBytes = Encoding.UTF8.GetBytes(message + NetworkSystem.overMark);
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
            Log($"UDPSend{remote}:{message}");
        }

        public void OpenUDP()
        {
            if (udpClient != null) return;
            while (true)
            {
                try
                {
                    udpClient = new UdpClient(Setting.clientUDPPort);
                    break;
                }
                catch (SocketException ex)
                {
                    Log(ex);
                    // TODO 处理异常
                    return;
                }
                catch (Exception ex)
                {
                    Log(ex);
                    CloseUDP();
                    return;
                }
            }
            udpReceiveThread = new Thread(UDPReceiveThread);
            udpReceiveThread.Start();
        }
        public void CloseUDP()
        {
            udpReceiveThread?.Abort();
            udpClient?.Close();
            udpClient = null;
        }

        /// <summary>
        /// 开始tcp连接，加入房间
        /// </summary>
        public void StartTCPConnecting()
        {
            while (true)
            {
                try
                {
                    client = new TcpClient(new IPEndPoint(NetworkSystem.LocalIPAddress, port));
                    connectThread = new Thread(ConnectThread);
                    connectThread.Start();
                    break;
                }
                catch (SocketException ex)
                {
                    Log(ex);
                    if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                    {
                        port = NetworkSystem.GetValidPort();
                    }
                    else break;
                }
                catch (Exception ex)
                {
                    Log(ex);
                    StopTCPConnecting();
                    return;
                }
            }
        }
        /// <summary>
        /// 结束tcp连接
        /// </summary>
        public void StopTCPConnecting()
        {
            if (IsConnected) NetworkSystem.CallDisconnection();
            connectThread?.Abort();
            receiveThread?.Abort();
            stream?.Close();
            client?.Close();
        }
        #endregion

        #region Inner Code -----------------------------------
        GameSystem.Setting.NetworkSystemSetting Setting { get { return NetworkSystem.Setting; } }
        int port;
        bool isDestroyed = false;

        public Client()
        {
            try
            {
                port = NetworkSystem.GetValidPort();
                Log("客户端已启用……:" + port);
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
        UdpClient udpClient;
        Thread udpReceiveThread;

        void UDPReceiveThread()
        {
            Log("开始收UDP包……:" + Setting.clientUDPPort);
            while (true)
            {
                try
                {
                    IPEndPoint remoteIP = new IPEndPoint(IPAddress.Any, Setting.clientUDPPort);
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

        TcpClient client;
        Thread connectThread;
        NetworkStream stream;
        Thread receiveThread;
        byte[] buffer = new byte[NetworkSystem.maxMsgLength];
        string receiveStringBuffer = "";
        Regex PacketCutter = new Regex(@"^([^✡]*)✡(.*)$");

        void ConnectThread()
        {
            do
            {
                Log($"Connecting……|ip:{NetworkSystem.LocalIPAddress}:{port}|remote:{NetworkSystem.ServerIPAddress}:{Setting.serverTCPPort}");
                try
                {
                    client.Connect(new IPEndPoint(NetworkSystem.ServerIPAddress, Setting.serverTCPPort));
                    // Block --------------------------------
                }
                catch (SocketException ex)
                {
                    Log(ex);
                    Log("连接失败！重新连接中……:" + port);
                    if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                    {
                        while (true)
                        {
                            client?.Close();
                            port = NetworkSystem.GetValidPort();
                            Log("更换Port重连中:" + port);
                            try
                            {
                                client = new TcpClient(new IPEndPoint(NetworkSystem.LocalIPAddress, port));
                                break;
                            }
                            catch (SocketException exx)
                            {
                                if (exx.SocketErrorCode == SocketError.AddressAlreadyInUse) continue;
                            }
                        }
                    }
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
        }
        void ReceiveThread()
        {
            string receiveString;

            int count;
            count = stream.Read(buffer, 0, buffer.Length);
            // Block --------------------------------
            if (count <= 0)
            {
                Log("与服务器断开连接");
                NetworkSystem.ShutdownClient();
                return;
            }
            receiveString = Encoding.UTF8.GetString(buffer, 0, count);
            NetworkSystem.netId = receiveString.Substring(0, receiveString.Length - 1);
            NetworkSystem.CallConnection();

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
                        return;
                    }
                    // TODO 得处理超长度的情况
                    receiveString = Encoding.UTF8.GetString(buffer, 0, count);
                    Log($"Receive{client.Client.LocalEndPoint}:{receiveString}");
                    receiveStringBuffer += receiveString;

                    Match match = PacketCutter.Match(receiveStringBuffer);
                    while (match.Success)
                    {
                        NetworkSystem.CallReceive(match.Groups[1].Value);
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
                NetworkSystem.ShutdownClient();
            }
        }
        #endregion

        void Log(object msg)
        {
            if (!TheMatrix.debug) return;
            string msgStr = "[Client]" + msg.ToString();
            NetworkSystem.CallLog(msgStr);
        }
        void Log(SocketException ex)
        {
            NetworkSystem.CallLog("[Client Exception:" + port + "]" + ex.GetType().Name + "|" + ex.SocketErrorCode + ":" + ex.Message + "\n" + ex.StackTrace);
        }
        void Log(Exception ex)
        {
            NetworkSystem.CallLog("[Client Exception]" + ex.GetType().Name + ":" + ex.Message + "\n" + ex.StackTrace);
        }
    }
}
