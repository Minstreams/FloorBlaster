using GameSystem.Networking;
using GameSystem.Networking.Packet;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace GameSystem
{
    namespace Operator
    {
        /// <summary>
        /// 大厅的UI控制
        /// </summary>
        [AddComponentMenu("[NetworkSystem]/Operator/LobbyManager")]
        public class LobbyManager : MonoBehaviour
        {
#if UNITY_EDITOR
            [MinsHeader("Operator of NetworkSystem", SummaryType.PreTitleOperator, -1)]
            [MinsHeader("联机大厅管理器", SummaryType.TitleOrange, 0)]
            [MinsHeader("大厅的UI控制", SummaryType.CommentCenter, 1)]
            [ConditionalShow, SerializeField] private bool useless; //在没有数据的时候让标题正常显示
#endif

            //Data
            //[MinsHeader("Data", SummaryType.Header, 2)]


            private Dictionary<IPEndPoint, UIRoomInfo> roomUIElements = new Dictionary<IPEndPoint, UIRoomInfo>();


            private void OnUDPReceive(UDPPacket packet)
            {
                var ep = packet.endPoint;
                var pkt = NetworkSystem.StringToPacket(packet.message);
                if (!pkt.MatchType(typeof(PacketRoomInfo))) return;
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
            }
            private void Awake()
            {
                NetworkSystem.OnUDPReceive += OnUDPReceive;
            }

            private void OnGUI()
            {
                foreach (var ro in roomUIElements)
                {
                    if (GUILayout.Button(ro.Value.Title))
                    {
                        NetworkSystem.ServerIPEndPoint = ro.Key;
                        TheMatrix.SendGameMessage(GameMessage.Next);
                    }
                }
            }

            //Input
            //[ContextMenu("SomeFuntion")]
            //public void SomeFuntion(){}
        }
    }
}