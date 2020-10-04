using UnityEngine;
using System.Collections;

namespace GameSystem
{
    namespace Networking
    {
        public abstract class TCPListener<pktType> : MonoBehaviour where pktType : PacketBase
        {
            private System.Action<PacketBase> receiveAction;
            private void Awake()
            {
                receiveAction = NetworkSystem.ListenForPacket<pktType>(OnReceivePacket); ;
            }
            private void OnDestroy()
            {
                NetworkSystem.StopListenForPacket<pktType>(receiveAction);
            }

            protected abstract void OnReceivePacket(pktType packet);
        }
    }
}