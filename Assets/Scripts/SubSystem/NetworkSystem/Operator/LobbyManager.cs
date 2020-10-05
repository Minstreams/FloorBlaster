using GameSystem.Networking;
using GameSystem.Networking.Packet;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;

namespace GameSystem
{
    namespace Operator
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

            [UDPReceive]
            private void UDPReceive(UDPPacket packet)
            {
                var pkt = NetworkSystem.StringToPacket(packet.message);
                if (pkt.MatchType(typeof(PacketRoomInfo)))
                {
                    var ep = packet.endPoint.Address;
                    var roomInfo = pkt as PacketRoomInfo;
                    if (roomUIElements.ContainsKey(ep))
                    {
                        roomUIElements[ep].Title = roomInfo.roomTitle;
                    }
                    else
                    {
                        var g = new GameObject(ep.ToString());
                        g.transform.SetParent(transform);
                        var el = g.AddComponent<UIRoomInfo>();
                        el.Title = roomInfo.roomTitle;
                        roomUIElements.Add(ep, el);
                    }
                    // 如果自己ip不确定，就向服务器发送回声检索
                    if (!NetworkSystem.localIPCheck) NetworkSystem.client.UDPSend(NetworkSystem.PacketToString(new PacketIPEcho(packet.endPoint.Address)), packet.endPoint);
                }
                else if (pkt.MatchType(typeof(PacketIPEcho)))
                {
                    if (NetworkSystem.isHost)
                    {
                        NetworkSystem.ServerIPAddress = NetworkSystem.LocalIPAddress;
                        TheMatrix.SendGameMessage(GameMessage.Next);
                    }
                }
            }

            private void Start()
            {
                NetworkSystem.client.OpenUDP();

            }
            protected override void OnDestroy()
            {
                base.OnDestroy();
                NetworkSystem.client.CloseUDP();
            }

            //Server========================================
            private bool ipOccupied = false;
            [ContextMenu("LaunchServer")]
            public void LaunchServer()
            {
                try
                {
                    NetworkSystem.LaunchServer();
                    RoomManager.currentRoomName = "默认房间名";
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

            private IEnumerator BoardcastInfo()
            {
                while (true)
                {
                    NetworkSystem.server.UDPBoardcast(NetworkSystem.PacketToString(new PacketRoomInfo(RoomManager.currentRoomName)));
                    yield return new WaitForSeconds(NetworkSystem.Setting.udpBoardcastInterval);
                }
            }

            private void OnGUI()
            {
                if (ipOccupied)
                {
                    GUILayout.Button("相同地址已经存在一个服务器~");
                }
                else if (GUILayout.Button("创建房间"))
                {
                    LaunchServer();
                }
                foreach (var ro in roomUIElements)
                {
                    if (GUILayout.Button(ro.Value.Title))
                    {
                        NetworkSystem.ServerIPAddress = ro.Key;
                        TheMatrix.SendGameMessage(GameMessage.Next);
                    }
                }
            }
        }
    }
}