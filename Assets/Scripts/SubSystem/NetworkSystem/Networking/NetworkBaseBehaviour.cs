using UnityEngine;
using System.Net;
using System.Reflection;

namespace GameSystem.Networking
{
    /// <summary>
    /// 基类，提供网络API，不要调用
    /// </summary>
    public abstract class NetworkBaseBehaviour : MonoBehaviour
    {
        protected static Setting.NetworkSystemSetting Setting => NetworkSystem.Setting;
        /// <summary>
        /// 是否是主机
        /// </summary>
        protected static bool IsServer => NetworkSystem.IsServer;
        /// <summary>
        /// 是否已连上主机
        /// </summary>
        protected static bool IsConnected => NetworkSystem.IsConnected;
        /// <summary>
        /// 本地IP是否已经确定
        /// </summary>
        protected static bool LocalIPCheck => NetworkSystem.LocalIPCheck;
        protected static IPAddress LocalIPAddress { get => NetworkSystem.LocalIPAddress; set => NetworkSystem.LocalIPAddress = value; }
        protected static IPAddress ServerIPAddress => NetworkSystem.ServerIPAddress;
        protected float timer { get => NetworkSystem.timer; set => NetworkSystem.timer = value; }
        protected float ServerTimer => NetworkSystem.ServerTimer;
        /// <summary>
        /// 调用主线程
        /// </summary>
        protected void CallMainThread(System.Action action) => NetworkSystem.CallMainThread(action);
        protected static PacketBase StringToPacket(string str) => NetworkSystem.StringToPacket(str);
        protected static string PacketToString(PacketBase pkt) => NetworkSystem.PacketToString(pkt);
        protected static void ClientSendPacket(PacketBase pkt) => NetworkSystem.ClientSendPacket(pkt);
        protected static void ClientUDPSendPacket(PacketBase pkt, IPEndPoint endPoint) => NetworkSystem.ClientUDPSendPacket(pkt, endPoint);
        protected static void ServerBoardcastPacket(PacketBase pkt) => NetworkSystem.ServerBoardcastPacket(pkt);
        protected static void ServerUDPSendPacket(PacketBase pkt, IPEndPoint endPoint) => NetworkSystem.ServerUDPSendPacket(pkt, endPoint);
        protected static void ServerUDPBoardcastPacket(PacketBase pkt) => NetworkSystem.ServerUDPBoardcastPacket(pkt);

        protected event System.Action _onDestroyEvent;
        protected virtual private void OnDestroy()
        {
            _onDestroyEvent?.Invoke();
        }

        /// <summary>
        /// 处理通用的Attribute
        /// </summary>
        protected bool _AttributeHandle(MethodInfo m)
        {
            if (m.GetCustomAttribute<UDPProcessAttribute>() != null)
            {
                _ParametersCheck<UDPProcessAttribute>(m, typeof(UDPPacket));
                System.Action<UDPPacket> mAction = (pkt) => { m.Invoke(this, new object[] { pkt }); };
                NetworkSystem.OnProcessUDPPacket += mAction;
                _onDestroyEvent += () => { NetworkSystem.OnProcessUDPPacket -= mAction; };
                return true;
            }
            else if (m.GetCustomAttribute<UDPReceiveAttribute>() != null)
            {
                _ParametersCheck<UDPReceiveAttribute>(m, typeof(UDPPacket));
                System.Action<UDPPacket> mAction = (pkt) => { m.Invoke(this, new object[] { pkt }); };
                NetworkSystem.OnUDPReceive += mAction;
                _onDestroyEvent += () => { NetworkSystem.OnUDPReceive -= mAction; };
                return true;
            }
            else if (m.GetCustomAttribute<TCPConnectionAttribute>() != null)
            {
                _ParametersCheck<TCPConnectionAttribute>(m);
                System.Action mAction = () => { m.Invoke(this, new object[] { }); };
                NetworkSystem.OnConnection += mAction;
                _onDestroyEvent += () => { NetworkSystem.OnConnection -= mAction; };
                return true;
            }
            else if (m.GetCustomAttribute<TCPDisconnectionAttribute>() != null)
            {
                _ParametersCheck<TCPDisconnectionAttribute>(m);
                System.Action mAction = () => { m.Invoke(this, new object[] { }); };
                NetworkSystem.OnDisconnection += mAction;
                _onDestroyEvent += () => { NetworkSystem.OnDisconnection -= mAction; };
                return true;
            }
            else if (m.GetCustomAttribute<TCPConnectionProcessAttribute>() != null)
            {
                _ParametersCheck<TCPConnectionProcessAttribute>(m, typeof(Server.Connection));
                System.Action<Server.Connection> mAction = (conn) => { m.Invoke(this, new object[] { conn }); };
                NetworkSystem.OnProcessConnection += mAction;
                _onDestroyEvent += () => { NetworkSystem.OnProcessConnection -= mAction; };
                return true;
            }
            else if (m.GetCustomAttribute<TCPDisconnectionProcessAttribute>() != null)
            {
                _ParametersCheck<TCPDisconnectionProcessAttribute>(m, typeof(Server.Connection));
                System.Action<Server.Connection> mAction = (conn) => { m.Invoke(this, new object[] { conn }); };
                NetworkSystem.OnProcessDisconnection += mAction;
                _onDestroyEvent += () => { NetworkSystem.OnProcessDisconnection -= mAction; };
                return true;
            }
            return false;
        }
        /// <summary>
        /// 查询Attribute对应的参数表列是否正确
        /// </summary>
        protected string _ParametersCheck<Attri>(MethodInfo m, params System.Type[] parameters)
        {
            var ps = m.GetParameters();
            bool check = true;
            if (ps.Length != parameters.Length) check = false;
            for (int i = 0; i < parameters.Length; ++i)
            {
                var pt = ps[i].ParameterType;
                if (!pt.Equals(parameters[i]) && !pt.IsSubclassOf(parameters[i])) check = false;
            }
            if (!check)
            {
                string errMsg = this.name + "." + m.Name + "方法不符合" + typeof(Attri).Name + "的参数要求。\n要求参数：(";
                for (int i = 0; i < parameters.Length; i++) errMsg += $"[{i}]{parameters[i].FullName},";
                errMsg += ")。\n实际参数：(";
                for (int i = 0; i < ps.Length; i++) errMsg += $"[{i}]{ps[i].ParameterType.FullName},";
                errMsg += ")。";
                throw new System.Exception(errMsg);
            }
            return ps.Length > 0 ? ps[0].ParameterType.FullName : "";
        }
    }
}