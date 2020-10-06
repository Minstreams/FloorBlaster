using System.Reflection;

namespace GameSystem.Networking
{
    /// <summary>
    /// 一般网络物体，有网络通信功能
    /// </summary>
    public abstract class NetworkObject : NetworkBaseBehaviour
    {
        protected virtual private void Awake()
        {
            var ms = this.GetType().GetRuntimeMethods();
            //Debug.Log("Start Processing. Type:" + this.GetType().FullName);
            foreach (var m in ms)
            {
                if (_AttributeHandle(m)) continue;
                if (m.GetCustomAttribute<TCPProcessAttribute>() != null)
                {
                    string pType = _ParametersCheck<TCPProcessAttribute>(m, typeof(PacketBase), typeof(Server.Connection));
                    System.Action<PacketBase, Server.Connection> mAction = (pktBase, Connection) => { m.Invoke(this, new object[] { pktBase, Connection }); };
                    NetworkSystem.ProcessPacket(pType, mAction);
                    _onDestroyEvent += () => { NetworkSystem.StopProcessPacket(pType, mAction); };
                }
                else if (m.GetCustomAttribute<TCPReceiveAttribute>() != null)
                {
                    string pType = _ParametersCheck<TCPReceiveAttribute>(m, typeof(PacketBase));
                    System.Action<PacketBase> mAction = (pktBase) => { m.Invoke(this, new object[] { pktBase }); };
                    NetworkSystem.ListenForPacket(pType, mAction);
                    _onDestroyEvent += () => { NetworkSystem.StopListenForPacket(pType, mAction); };
                }
            }
        }
    }
}