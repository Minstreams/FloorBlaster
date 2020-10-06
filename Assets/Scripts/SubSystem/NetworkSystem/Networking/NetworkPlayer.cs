using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using GameSystem.Networking.Packet;

namespace GameSystem.Networking
{
    /// <summary>
    /// 带有netId标识的物体，管理与服务器的单独通信,
    /// </summary>
    public abstract class NetworkPlayer : NetworkBaseBehaviour
    {
        /// <summary>
        /// 当前物体的网络ID
        /// </summary>
        public string netId = "0";
        public bool initializeOnAwake = false;
        event System.Action onDestroyEvent;
        protected virtual private void OnDestroy()
        {
            onDestroyEvent?.Invoke();
        }
        /// <summary>
        /// 服务器处理消息
        /// </summary>
        private static Dictionary<string, System.Func<object, object[], object>> tcpProcessors = new Dictionary<string, System.Func<object, object[], object>>();
        /// <summary>
        /// 客户端接收消息
        /// </summary>
        private Dictionary<string, System.Func<object, object[], object>> tcpDistributors = new Dictionary<string, System.Func<object, object[], object>>();

        private void _TCPProcess(PacketBase packet, Server.Connection connection)
        {
            string tp = packet.pktTypeStr;
            if (tcpProcessors.ContainsKey(tp)) tcpProcessors[tp]?.Invoke(this, new object[] { packet, connection });
        }
        private void _TCPReceive(PacketToId packet)
        {
            string tp = packet.pktTypeStr;
            if (tcpProcessors.ContainsKey(tp)) tcpProcessors[tp]?.Invoke(this, new object[] { packet });
        }

        /// <summary>
        /// 必须设置netId后手动调用此方法
        /// </summary>
        [ContextMenu("Initialize")]
        public void Initialize()
        {
            NetworkSystem.ProcessPacketFromId(netId, _TCPProcess);
            NetworkSystem.ListenForPacketToId(netId, _TCPReceive);
            onDestroyEvent += () =>
            {
                NetworkSystem.StopProcessPacketFromId(netId, _TCPProcess);
                NetworkSystem.StopListenForPacketToId(netId, _TCPReceive);
            };

            var ms = this.GetType().GetRuntimeMethods();
            //Debug.Log("Start Processing. Type:" + this.GetType().FullName);
            foreach (var m in ms)
            {

                if (m.GetCustomAttribute<TCPProcessAttribute>() != null)
                {
                    var ps = m.GetParameters();
                    if (ps.Length != 2 || !ps[0].ParameterType.IsSubclassOf(typeof(PacketBase)) || ps[1].ParameterType != typeof(Server.Connection))
                    {
                        string errMsg = m.Name + "方法不符合TCPProcess的参数要求。参数：";
                        for (int i = 0; i < ps.Length; i++) errMsg += $"[{i}]{ps[i].ParameterType.FullName}";
                        throw new System.Exception(errMsg);
                    }
                    string tp = ps[0].ParameterType.FullName;
                    if (!tcpProcessors.ContainsKey(tp)) tcpProcessors.Add(tp, null);
                    tcpProcessors[tp] += m.Invoke;
                    //Debug.Log(m.Name + "|TCPProcessAttribute");
                }
                else if (m.GetCustomAttribute<UDPProcessAttribute>() != null)
                {
                    var ps = m.GetParameters();
                    if (ps.Length != 1 || ps[0].ParameterType != typeof(UDPPacket))
                    {
                        string errMsg = m.Name + "方法不符合UDPProcess的参数要求。参数：";
                        for (int i = 0; i < ps.Length; i++) errMsg += $"[{i}]{ps[i].ParameterType.FullName}";
                        throw new System.Exception(errMsg);
                    }
                    System.Action<UDPPacket> mAction = (pkt) => { m.Invoke(this, new object[] { pkt }); };
                    NetworkSystem.OnProcessUDPPacket += mAction;
                    onDestroyEvent += () => { NetworkSystem.OnProcessUDPPacket -= mAction; };
                    //Debug.Log(m.Name + "|UDPProcessAttribute");
                }
                else if (m.GetCustomAttribute<TCPReceiveAttribute>() != null)
                {
                    var ps = m.GetParameters();
                    if (ps.Length != 1 || !ps[0].ParameterType.IsSubclassOf(typeof(PacketToId)))
                    {
                        string errMsg = m.Name + "方法不符合TCPReceive的参数要求。参数：";
                        for (int i = 0; i < ps.Length; i++) errMsg += $"[{i}]{ps[i].ParameterType.FullName}";
                        throw new System.Exception(errMsg);
                    }
                    string tp = ps[0].ParameterType.FullName;
                    if (!tcpDistributors.ContainsKey(tp)) tcpDistributors.Add(tp, null);
                    tcpDistributors[tp] += m.Invoke;
                    //Debug.Log(m.Name + "|TCPReceiveAttribute");
                }
                else if (m.GetCustomAttribute<UDPReceiveAttribute>() != null)
                {
                    var ps = m.GetParameters();
                    if (ps.Length != 1 || ps[0].ParameterType != typeof(UDPPacket))
                    {
                        string errMsg = m.Name + "方法不符合UDPReceive的参数要求。参数：";
                        for (int i = 0; i < ps.Length; i++) errMsg += $"[{i}]{ps[i].ParameterType.FullName}";
                        throw new System.Exception(errMsg);
                    }
                    System.Action<UDPPacket> mAction = (pkt) => { m.Invoke(this, new object[] { pkt }); };
                    NetworkSystem.OnUDPReceive += mAction;
                    onDestroyEvent += () => { NetworkSystem.OnUDPReceive -= mAction; };
                    //Debug.Log(m.Name + "|UDPReceiveAttribute");
                }
            }
        }
        protected virtual private void Awake()
        {
            if (initializeOnAwake) Initialize();
        }
    }
}
