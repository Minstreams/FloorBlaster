using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Setting;
using GameSystem.Networking;
using System.Net;

namespace GameSystem
{

    /// <summary>
    /// 网络系统。用于多人联机和服务器服务。
    /// </summary>
    public class NetworkSystem : SubSystem<NetworkSystemSetting>
    {
        public const int maxMsgLength = 2048;
        public const string clientHello = "Hellov0.0.1";
        public const string serverHi = "Hi~";
        public static bool isHost { get; private set; }



        /// <summary>
        /// Call Debug.Log in Main Thread
        /// </summary>
        /// <param name="message"></param>
        private static Queue<string> pendingLogQueue = new Queue<string>();
        private static bool pendingShutdownServer = false;
        private static bool pendingShutdownClient = false;
        private static Queue<UDPPacket> pendingUDPReceiveQueue = new Queue<UDPPacket>();
        private static Queue<string> pendingReceiveQueue = new Queue<string>();
        private static bool pendingConnected = false;
        private static bool pendingDisconnected = false;
        private static Dictionary<string, System.Action<PacketBase>> tcpDistributors = new Dictionary<string, System.Action<PacketBase>>();
        private static Dictionary<string, System.Action<PacketBase>> tcpProcessors = new Dictionary<string, System.Action<PacketBase>>();
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
                }
                if (pendingConnected) { pendingConnected = false; OnConnected?.Invoke(); }
                if (pendingDisconnected) { pendingDisconnected = false; OnDisconnected?.Invoke(); }
            }
        }



        // API ---------------------------------
        public static Server server = null;
        public static Client client = null;
        public static string LocalIP
        {
            get
            {
                return Setting.localIP;
            }
        }

        private static string serverIP = "";
        public static string ServerIP
        {
            get => serverIP;
            set
            {
                serverIP = value;
                serverEndPoint.Address = IPAddress.Parse(value);
            }
        }

        private static IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, Setting.serverTCPPort);
        public static IPEndPoint ServerIPEndPoint
        {
            get => serverEndPoint;
            set
            {
                serverEndPoint.Address = value.Address;
                serverIP = value.Address.ToString();
            }
        }
        public static void LaunchServer()
        {
            isHost = true;
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
            isHost = false;
            pendingShutdownServer = true;
        }
        public static void ShutdownClient()
        {
            pendingShutdownClient = true;
        }
        /// <summary>
        /// 获取两个可用端口分配给客户端
        /// 返回tcp端口
        /// udp端口 = tcp端口-1
        /// </summary>
        public static int GetValidPort()
        {
            //todo
            return Random.Range(Setting.minClientPort, Setting.maxClientPort) + 1;
        }
        /// <summary>
        /// 获取所有可能的服务器IP
        /// </summary>
        public static string[] GetPossibleIPs()
        {
            return new string[] { Setting.serverIP };
        }

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
        public static void ListenForPacket(string typeStr, System.Action<PacketBase> listener)
        {
            if (!tcpDistributors.ContainsKey(typeStr)) tcpDistributors.Add(typeStr, null);
            tcpDistributors[typeStr] += listener;
        }
        public static System.Action<PacketBase> ListenForPacket<pktType>(System.Action<pktType> listener) where pktType : PacketBase
        {
            string typeStr = typeof(pktType).FullName;
            if (!tcpDistributors.ContainsKey(typeStr)) tcpDistributors.Add(typeStr, null);
            System.Action<PacketBase> output = pkt => listener.Invoke(pkt as pktType);
            tcpDistributors[typeStr] += output;
            return output;
        }
        public static void StopListenForPacket(string typeStr, System.Action<PacketBase> listener)
        {
            if (tcpDistributors.ContainsKey(typeStr))
            {
                tcpDistributors[typeStr] -= listener;
            }
        }
        /// <summary>
        /// 结束监听，必须输入开始监听时获取的Action对象
        /// </summary>
        public static void StopListenForPacket<pktType>(System.Action<PacketBase> listener) where pktType : PacketBase
        {
            string typeStr = typeof(pktType).FullName;
            if (tcpDistributors.ContainsKey(typeStr))
            {
                tcpDistributors[typeStr] -= pkt => listener.Invoke(pkt as pktType);
            }
        }
        /// <summary>
        /// 客户端注册Process事件
        /// </summary>
        public static void ProcessPacket(string typeStr, System.Action<PacketBase> processor)
        {
            if (!tcpProcessors.ContainsKey(typeStr)) tcpProcessors.Add(typeStr, null);
            tcpProcessors[typeStr] += processor;
        }
        /// <summary>
        /// 客户端注册Process事件
        /// </summary>
        public static System.Action<PacketBase> ProcessPacket<pktType>(System.Action<pktType> processor) where pktType : PacketBase
        {
            string typeStr = typeof(pktType).FullName;
            if (!tcpProcessors.ContainsKey(typeStr)) tcpProcessors.Add(typeStr, null);
            System.Action<PacketBase> output = pkt => processor.Invoke(pkt as pktType);
            tcpProcessors[typeStr] += output;
            return output;
        }
        public static void StopProcessPacket(string typeStr, System.Action<PacketBase> processor)
        {
            if (tcpProcessors.ContainsKey(typeStr))
            {
                tcpProcessors[typeStr] -= processor;
            }
        }
        /// <summary>
        /// 结束服务器处理，必须输入开始服务器处理时获取的Action对象
        /// </summary>
        public static void StopProcessPacket<pktType>(System.Action<PacketBase> processor) where pktType : PacketBase
        {
            string typeStr = typeof(pktType).FullName;
            if (tcpProcessors.ContainsKey(typeStr))
            {
                tcpProcessors[typeStr] -= processor;
            }
        }
        /// <summary>
        /// 给服务器送Packet
        /// </summary>
        public static void SendPacket(PacketBase pkt)
        {
            client?.Send(PacketToString(pkt));
        }
        public static void ServerBoardcastPacket(PacketBase pkt)
        {
            server?.Boardcast(PacketToString(pkt));
        }

        // Events ------------------------------
        public static event System.Action<UDPPacket> OnUDPReceive;
        public static event System.Action<string> OnReceive;
        public static event System.Action OnConnected;
        public static event System.Action OnDisconnected;

        // For Servers & Client ----------------
        public static void CallLog(string message)
        {
            pendingLogQueue.Enqueue(message);
        }
        public static void InvokeUDPReceive(UDPPacket packet)
        {
            pendingUDPReceiveQueue.Enqueue(packet);
        }
        public static void InvokeReceive(string message)
        {
            pendingReceiveQueue.Enqueue(message);
        }
        public static void InvokeConnected()
        {
            pendingConnected = true;
        }
        public static void InvokeDisconnected()
        {
            pendingDisconnected = true;
        }
        public static void CallProcessPacket(string pktMessage)
        {
            PacketBase pkt = StringToPacket(pktMessage);
            if (tcpProcessors.ContainsKey(pkt.pktTypeStr))
            {
                tcpProcessors[pkt.pktTypeStr]?.Invoke(pkt);
            }
        }


        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInit()
        {
            isHost = false;
            //用于控制Action初始化
            TheMatrix.onGameAwake += OnGameAwake;
            TheMatrix.onGameStart += OnGameStart;
            Application.quitting += OnGameQuitting;
            //随便找个Setting里的值，用于在分线程前提前初始化Setting
            int active = Setting.serverTCPPort;
        }
        private static void OnGameAwake()
        {
            StartCoroutine(MainThread());
        }
        private static void OnGameStart()
        {
            //在主场景游戏开始时和游戏重新开始时调用
        }
        private static void OnGameQuitting()
        {
            ShutdownClient();
            ShutdownServer();
        }
    }
}
