using GameSystem.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace GameSystem.Operator
{
    /// <summary>
    /// 大厅的UI控制
    /// </summary>
    [AddComponentMenu("[NetworkSystem]/Operator/LobbyManager")]
    public class LobbyManager : NetworkObject
    {
#if UNITY_EDITOR
        [MinsHeader("Operator of NetworkSystem", SummaryType.PreTitleOperator, -1)]
        [MinsHeader("联机大厅管理器", SummaryType.TitleOrange, 0)]
        [MinsHeader("大厅的UI控制", SummaryType.CommentCenter, 1)]
        [ConditionalShow, SerializeField] private bool useless; //在没有数据的时候让标题正常显示
#endif
        private Dictionary<IPAddress, UIRoomInfo> roomUIElements = new Dictionary<IPAddress, UIRoomInfo>();
        private bool ipOccupied = false;

        private void Start()
        {
            NetworkSystem.client.OpenUDP();

        }
        protected override private void OnDestroy()
        {
            NetworkSystem.client.CloseUDP();
            base.OnDestroy();
        }

        [UDPReceive]
        void UDPReceive(UDPPacket packet)
        {
            var pkt = StringToPacket(packet.message);
            if (pkt.MatchType(typeof(URoomBrief)))
            {
                var ep = packet.endPoint.Address;
                var roomInfo = pkt as URoomBrief;
                if (roomInfo.hello != NetworkSystem.clientHello) return;    //版本不一致
                if (roomUIElements.ContainsKey(ep))
                {
                    roomUIElements[ep].Title = roomInfo.title;
                }
                else
                {
                    var g = new GameObject(ep.ToString());
                    g.transform.SetParent(transform);
                    var el = g.AddComponent<UIRoomInfo>();
                    el.Title = roomInfo.title;
                    roomUIElements.Add(ep, el);
                }
                if ((IsServer && !IsConnected) || !LocalIPCheck)
                {
                    // 新建房间时发送定位Echo
                    // 或者不确定自己地址时发送查询Echo
                    ClientUDPSendPacket(new UEcho(packet.endPoint.Address), packet.endPoint);
                }
            }
            else if (pkt.MatchType(typeof(UEcho)))
            {
                // 客户端收到Echo，确定自己的地址
                if (LocalIPCheck) return;
                UEcho pktEcho = pkt as UEcho;
                LocalIPAddress = pktEcho.address;
            }
        }
        [UDPProcess]
        void UDPPRocess(UDPPacket packet)
        {
            PacketBase pkt = StringToPacket(packet.message);
            if (pkt.MatchType(typeof(UEcho)))
            {
                IPAddress addr = (pkt as UEcho).address;

                if (addr.Equals(packet.endPoint.Address) && !NetworkSystem.server.TcpOn)
                {
                    // 是本地发来的，打开tcp监听，并进入本地房间
                    LocalIPAddress = addr;
                    NetworkSystem.server.TurnOnTCP();
                    NetworkSystem.ConnectTo(addr);
                }
            }
        }


        /// <summary>
        /// 作为主机开房间
        /// </summary>
        [ContextMenu("LaunchServer")]
        public void LaunchServer()
        {
            try
            {
                NetworkSystem.LaunchServer();
                StartCoroutine(BoardcastInfo());
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    NetworkSystem.ShutdownServer();
                    ipOccupied = true;
                }
            }
        }
        IEnumerator BoardcastInfo()
        {
            while (true)
            {
                ServerUDPBoardcastPacket(new URoomBrief(PersonalizationSystem.LocalRoomInfo.name));
                yield return new WaitForSeconds(Setting.udpBoardcastInterval);
            }
        }

        void OnGUI()
        {
            if (ipOccupied)
            {
                GUILayout.Button("相同地址已经存在一个服务器~");
            }
            else if (GUILayout.Button("创建房间"))
            {
                LaunchServer();
            }
            if (LocalIPCheck)
            {
                foreach (var ro in roomUIElements)
                {
                    if (GUILayout.Button(ro.Value.Title))
                    {
                        NetworkSystem.ConnectTo(ro.Key);
                    }
                }
            }
        }
    }
}