using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

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

        /// <summary>
        /// 服务器处理消息
        /// </summary>
        static Dictionary<string, System.Func<object, object[], object>> tcpProcessors = new Dictionary<string, System.Func<object, object[], object>>();
        /// <summary>
        /// 客户端接收消息
        /// </summary>
        Dictionary<string, System.Func<object, object[], object>> tcpDistributors = new Dictionary<string, System.Func<object, object[], object>>();

        void _TCPProcess(PacketBase packet, Server.Connection connection)
        {
            string tp = packet.pktTypeStr;
            if (tcpProcessors.ContainsKey(tp)) tcpProcessors[tp]?.Invoke(this, new object[] { packet, connection });
        }
        void _TCPReceive(Pktid packet)
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
            _onDestroyEvent += () =>
            {
                NetworkSystem.StopProcessPacketFromId(netId, _TCPProcess);
                NetworkSystem.StopListenForPacketToId(netId, _TCPReceive);
            };

            var ms = this.GetType().GetRuntimeMethods();
            //Debug.Log("Start Processing. Type:" + this.GetType().FullName);
            foreach (var m in ms)
            {
                if (_AttributeHandle(m)) continue;
                if (m.GetCustomAttribute<TCPProcessAttribute>() != null)
                {
                    string pType = _ParametersCheck<TCPProcessAttribute>(m, typeof(PacketBase), typeof(Server.Connection));
                    if (!tcpProcessors.ContainsKey(pType)) tcpProcessors.Add(pType, null);
                    tcpProcessors[pType] += m.Invoke;
                }
                else if (m.GetCustomAttribute<TCPReceiveAttribute>() != null)
                {
                    string pType = _ParametersCheck<TCPReceiveAttribute>(m, typeof(Pktid));
                    if (!tcpDistributors.ContainsKey(pType)) tcpDistributors.Add(pType, null);
                    tcpDistributors[pType] += m.Invoke;
                }
            }
        }
        protected virtual private void Awake()
        {
            if (initializeOnAwake) Initialize();
        }
    }
}
