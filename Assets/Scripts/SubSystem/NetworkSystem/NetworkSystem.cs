using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Setting;
using GameSystem.Networking;
using System.Net;
using GameSystem.Networking.Packet;

namespace GameSystem
{

    /// <summary>
    /// 网络系统。用于多人联机和服务器服务。
    /// </summary>
    public class NetworkSystem : SubSystem<NetworkSystemSetting>
    {
        /// <summary>
        /// TCP buffer 最大长度
        /// </summary>
        public const int maxMsgLength = 2048;
        /// <summary>
        /// 寻找主机时使用的验证消息
        /// </summary>
        public const string clientHello = "Hellov0.0.1";

        /// <summary>
        /// 是否是主机
        /// </summary>
        public static bool IsServer { get; private set; } = false;
        /// <summary>
        /// 是否已连上主机
        /// </summary>
        public static bool IsConnected => client != null && client.IsConnected;
        /// <summary>
        /// 当前的网络ID
        /// </summary>
        public static string netId;

        /// <summary>
        /// 本地IP是否已经确定
        /// </summary>
        public static bool LocalIPCheck { private set; get; } = false;
        public static IPAddress LocalIPAddress
        {
            get => localIPAddress;
            set
            {
                localIPAddress = value;
                LocalIPCheck = true;
            }
        }
        static IPAddress localIPAddress = IPAddress.Any;
        public static IPAddress ServerIPAddress { get; private set; } = IPAddress.Any;

        public static Server server = null;
        public static Client client = null;

        #region API ------------------------------------------
        /// <summary>
        /// 获取tcp可用端口分配给客户端
        /// 返回tcp端口
        /// </summary>
        public static int GetValidPort()
        {
            return Random.Range(Setting.minClientPort, Setting.maxClientPort + 1);
        }
        // Packet 封包解包
        public static PacketBase StringToPacket(string str)
        {
            try
            {
                PacketBase temp = JsonUtility.FromJson(str, typeof(PacketBase)) as PacketBase;
                return JsonUtility.FromJson(str, temp.pktType) as PacketBase;
            }
            catch (System.Exception ex)
            {
                LogError(ex);
                LogError("sting:" + str);
            }
            return null;
        }
        public static string PacketToString(PacketBase pkt)
        {
            return JsonUtility.ToJson(pkt);
        }

        // 服务器客户端控制
        public static void LaunchServer()
        {
            IsServer = true;
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
            IsServer = false;
            pendingShutdownServer = true;
        }
        public static void ShutdownClient()
        {
            pendingShutdownClient = true;
        }
        public static void ConnectTo(IPAddress serverIPAddress)
        {
            ServerIPAddress = serverIPAddress;
            client?.StartTCPConnecting();
        }

        // tcp控制
        /// <summary>
        /// 给服务器送Packet
        /// </summary>
        public static void ClientSendPacket(PacketBase pkt)
        {
            client?.Send(PacketToString(pkt));
        }
        public static void ServerBoardcastPacket(PacketBase pkt)
        {
            server?.Boardcast(PacketToString(pkt));
        }

        // udp控制
        public static void ClientUDPSendPacket(PacketBase pkt, IPEndPoint endPoint)
        {
            client?.UDPSend(PacketToString(pkt), endPoint);
        }
        public static void ServerUDPSendPacket(PacketBase pkt, IPEndPoint endPoint)
        {
            server?.UDPSend(PacketToString(pkt), endPoint);
        }
        public static void ServerUDPBoardcastPacket(PacketBase pkt)
        {
            server?.UDPBoardcast(PacketToString(pkt));
        }


        // 数据处理事件注册方法
        public static void ListenForPacket(string typeStr, System.Action<PacketBase> listener)
        {
            if (!tcpDistributors.ContainsKey(typeStr)) tcpDistributors.Add(typeStr, null);
            tcpDistributors[typeStr] += listener;
        }
        public static void StopListenForPacket(string typeStr, System.Action<PacketBase> listener)
        {
            if (tcpDistributors.ContainsKey(typeStr))
            {
                tcpDistributors[typeStr] -= listener;
            }
        }
        public static void ListenForPacketToId(string id, System.Action<PacketToId> listener)
        {
            if (!tcpSubDistributors.ContainsKey(id)) tcpSubDistributors.Add(id, null);
            tcpSubDistributors[id] += listener;
        }
        public static void StopListenForPacketToId(string id, System.Action<PacketToId> listener)
        {
            if (tcpSubDistributors.ContainsKey(id))
            {
                tcpSubDistributors[id] -= listener;
            }
        }
        public static void ProcessPacket(string typeStr, System.Action<PacketBase, Server.Connection> processor)
        {
            if (!tcpProcessors.ContainsKey(typeStr)) tcpProcessors.Add(typeStr, null);
            tcpProcessors[typeStr] += processor;
        }
        public static void StopProcessPacket(string typeStr, System.Action<PacketBase, Server.Connection> processor)
        {
            if (tcpProcessors.ContainsKey(typeStr))
            {
                tcpProcessors[typeStr] -= processor;
            }
        }
        public static void ProcessPacketFromId(string id, System.Action<PacketBase, Server.Connection> processor)
        {
            if (!tcpSubProcessors.ContainsKey(id)) tcpSubProcessors.Add(id, null);
            tcpSubProcessors[id] += processor;
        }
        public static void StopProcessPacketFromId(string id, System.Action<PacketBase, Server.Connection> processor)
        {
            if (tcpSubProcessors.ContainsKey(id))
            {
                tcpSubProcessors[id] -= processor;
            }
        }
        #endregion

        #region Events ---------------------------------------

        public static event System.Action<UDPPacket> OnUDPReceive;
        public static event System.Action<string> OnReceive;
        public static event System.Action OnConnected;
        public static event System.Action OnDisconnected;
        public static event System.Action<UDPPacket> OnProcessUDPPacket;
        public static event System.Action<string, Server.Connection> OnProcess;

        #endregion

        #region For Servers & Client -------------------------
        // Both
        /// <summary>
        /// Call Debug.Log in Main Thread
        /// </summary>
        public static void CallLog(string message)
        {
            pendingLogQueue.Enqueue(message);
            /// <summary>
            /// Call Debug.Log in Main Thread
            /// </summary>
            /// <param name="message"></param>
        }

        // 客户端
        public static void CallUDPReceive(UDPPacket packet)
        {
            pendingUDPReceiveQueue.Enqueue(packet);
        }
        public static void CallReceive(string message)
        {
            pendingReceiveQueue.Enqueue(message);
        }
        public static void CallConnected()
        {
            pendingConnected = true;
        }
        public static void CallDisconnected()
        {
            pendingDisconnected = true;
        }

        // 服务端
        public static void CallProcessPacket(string message, Server.Connection connection)
        {
            PacketBase pkt = StringToPacket(message);
            OnProcess?.Invoke(message, connection);
            if (tcpProcessors.ContainsKey(pkt.pktTypeStr))
            {
                tcpProcessors[pkt.pktTypeStr]?.Invoke(pkt, connection);
            }
            if (tcpSubProcessors.ContainsKey(connection.netId))
            {
                tcpSubProcessors[connection.netId]?.Invoke(pkt, connection);
            }
        }
        public static void CallProcessUDPPacket(UDPPacket packet)
        {
            OnProcessUDPPacket?.Invoke(packet);
        }

        #endregion

        #region Inner Code -----------------------------------
        /// <summary>
        /// 客户端接收消息
        /// </summary>
        static Dictionary<string, System.Action<PacketBase>> tcpDistributors = new Dictionary<string, System.Action<PacketBase>>();
        /// <summary>
        /// 客户端根据ID筛选接收消息
        /// </summary>
        static Dictionary<string, System.Action<PacketToId>> tcpSubDistributors = new Dictionary<string, System.Action<PacketToId>>();
        /// <summary>
        /// 服务器处理消息
        /// </summary>
        static Dictionary<string, System.Action<PacketBase, Server.Connection>> tcpProcessors = new Dictionary<string, System.Action<PacketBase, Server.Connection>>();
        /// <summary>
        /// 服务器根据ID筛选处理
        /// </summary>
        static Dictionary<string, System.Action<PacketBase, Server.Connection>> tcpSubProcessors = new Dictionary<string, System.Action<PacketBase, Server.Connection>>();

        static readonly Queue<string> pendingLogQueue = new Queue<string>();
        static bool pendingShutdownServer = false;
        static bool pendingShutdownClient = false;
        static readonly Queue<UDPPacket> pendingUDPReceiveQueue = new Queue<UDPPacket>();
        static readonly Queue<string> pendingReceiveQueue = new Queue<string>();
        static bool pendingConnected = false;
        static bool pendingDisconnected = false;
        static IEnumerator MainThread()
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
                while (pendingUDPReceiveQueue.Count > 0) OnUDPReceive?.Invoke(pendingUDPReceiveQueue.Dequeue());
                while (pendingReceiveQueue.Count > 0)
                {
                    string msg = pendingReceiveQueue.Dequeue();
                    OnReceive?.Invoke(msg);
                    var pkt = StringToPacket(msg);
                    if (tcpDistributors.ContainsKey(pkt.pktTypeStr))
                    {
                        tcpDistributors[pkt.pktTypeStr]?.Invoke(pkt);
                    }
                    if (pkt.IsSubclassOf(typeof(PacketToId)))
                    {
                        var pktTid = pkt as PacketToId;
                        if (tcpSubDistributors.ContainsKey(pktTid.id))
                        {
                            tcpSubDistributors[pktTid.id]?.Invoke(pktTid);
                        }
                    }
                }
                if (pendingConnected) { pendingConnected = false; TheMatrix.SendGameMessage(GameMessage.Connect); OnConnected?.Invoke(); }
                if (pendingDisconnected) { pendingDisconnected = false; TheMatrix.SendGameMessage(GameMessage.DisConnect); OnDisconnected?.Invoke(); }
            }
        }
        #endregion

        [RuntimeInitializeOnLoadMethod]
        static void RuntimeInit()
        {
            //用于控制Action初始化
            TheMatrix.onGameAwake += OnGameAwake;
            TheMatrix.onGameReady += OnGameReady;
            TheMatrix.onGameStart += OnGameStart;
            TheMatrix.onQuitting += OnGameQuitting;
            //随便找个Setting里的值，用于在分线程前提前初始化Setting
            int active = Setting.serverTCPPort;
        }
        static void OnGameAwake()
        {
            StartCoroutine(MainThread());
        }
        static void OnGameReady()
        {

        }
        static void OnGameStart()
        {
            //在主场景游戏开始时和游戏重新开始时调用
        }
        static void OnGameQuitting()
        {
            ShutdownServer();
            ShutdownClient();
        }
    }
}
