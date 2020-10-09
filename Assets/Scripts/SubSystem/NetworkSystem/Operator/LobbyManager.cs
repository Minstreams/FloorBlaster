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

        private void Start()
        {
            NetworkSystem.client.OpenUDP();
            ClientUDPSendPacket(new UIp(), new IPEndPoint(IPAddress.Broadcast, Setting.clientUDPPort));
            RefreshServerList();
        }
        protected override private void OnDestroy()
        {
            NetworkSystem.client.CloseUDP();
            base.OnDestroy();
        }

        public void RefreshServerList()
        {
            ClientUDPSendPacket(new HReqList(), NetworkSystem.HelperEndPoint);
        }

        [UDPReceive]
        void UDPReceive(UDPPacket packet)
        {
            var pkt = StringToPacket(packet.message);
            if (pkt.MatchType(typeof(UIp)))
            {
                // 客户端收到UIp,发回地址
                ClientUDPSendPacket(new UEcho(packet.endPoint.Address), packet.endPoint);
            }
            else if (pkt.MatchType(typeof(UEcho)))
            {
                // 客户端收到Echo，确定自己的地址
                UEcho pktEcho = pkt as UEcho;
                LocalIPAddress = pktEcho.address;
            }
            else if (pkt.MatchType(typeof(HServerList)))
            {
                // 刷新服务器表列
                var sList = pkt as HServerList;
                foreach (string sip in sList.sList)
                {
                    ClientUDPSendPacket(new UHello(), new IPEndPoint(IPAddress.Parse(sip), Setting.serverUDPPort));
                }
            }
            else if (pkt.MatchType(typeof(URoomBrief)))
            {
                // 更新服务器UI
                var ep = packet.endPoint.Address;
                var roomInfo = pkt as URoomBrief;
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
                NetworkSystem.server.TurnOnTCP();
                NetworkSystem.ConnectTo(NetworkSystem.LocalIPAddress);
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    NetworkSystem.ShutdownServer();
                    NotificationSystem.ShowNotification("相同地址已经存在一个服务器");
                }
            }
        }

        string localIp = "";
        string serverIp = "";
        void OnGUI()
        {
            if (GUILayout.Button("创建房间"))
            {
                LaunchServer();

            }
            localIp = GUILayout.TextField(localIp);
            serverIp = GUILayout.TextField(serverIp);
            if (GUILayout.Button("连接"))
            {
                NetworkSystem.LocalIPAddress = IPAddress.Parse(localIp);
                NetworkSystem.ConnectTo(IPAddress.Parse(serverIp));
            }
            if (GUILayout.Button("刷新"))
            {
                RefreshServerList();
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