using UnityEngine;
using System.Reflection;
using GameSystem.Networking.Packet;

namespace GameSystem
{
    namespace Networking
    {
        /// <summary>
        /// 一般网络物体，有网络通信功能
        /// </summary>
        public class NetworkObject : MonoBehaviour
        {
            event System.Action onDestroyEvent;
            protected virtual void OnDestroy()
            {
                onDestroyEvent?.Invoke();
            }
            protected virtual void Awake()
            {
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
                        System.Action<PacketBase, Server.Connection> mAction = (pktBase, Connection) => { m.Invoke(this, new object[] { pktBase, Connection }); };
                        NetworkSystem.ProcessPacket(ps[0].ParameterType.FullName, mAction);
                        onDestroyEvent += () => { NetworkSystem.StopProcessPacket(ps[0].ParameterType.FullName, mAction); };
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
                        if (ps.Length != 1 || !ps[0].ParameterType.IsSubclassOf(typeof(PacketBase)))
                        {
                            string errMsg = m.Name + "方法不符合TCPReceive的参数要求。参数：";
                            for (int i = 0; i < ps.Length; i++) errMsg += $"[{i}]{ps[i].ParameterType.FullName}";
                            throw new System.Exception(errMsg);
                        }
                        System.Action<PacketBase> mAction = (pktBase) => { m.Invoke(this, new object[] { pktBase }); };
                        NetworkSystem.ListenForPacket(ps[0].ParameterType.FullName, mAction);
                        onDestroyEvent += () => { NetworkSystem.StopListenForPacket(ps[0].ParameterType.FullName, mAction); };
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
        }
    }
}