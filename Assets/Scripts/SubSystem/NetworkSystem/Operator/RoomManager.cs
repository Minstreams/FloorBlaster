using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Networking;

namespace GameSystem.Operator
{
    /// <summary>
    /// 用来管理房间界面的网络数据传输
    /// </summary>
    [AddComponentMenu("[NetworkSystem]/Operator/RoomManager")]
    public class RoomManager : NetworkObject
    {
#if UNITY_EDITOR
        [MinsHeader("Operator of NetworkSystem", SummaryType.PreTitleOperator, -1)]
        [MinsHeader("房间管理器", SummaryType.TitleOrange, 0)]
        [MinsHeader("用来管理房间界面的网络数据传输", SummaryType.CommentCenter, 1)]
        [ConditionalShow, SerializeField] private bool useless; //在没有数据的时候让标题正常显示
#endif

        public static string currentRoomName;
        void Start()
        {
            if (IsServer)
            {
                StartCoroutine(BoardcastInfo());
            }
        }

        IEnumerator BoardcastInfo()
        {
            while (true)
            {
                ServerUDPBoardcastPacket(new PkRoomBrief(currentRoomName));
                yield return new WaitForSeconds(Setting.udpBoardcastInterval);
            }
        }

        [UDPProcess]
        void UDPPRocess(UDPPacket packet)
        {
            PacketBase pkt = StringToPacket(packet.message);
            if (pkt.MatchType(typeof(PkEcho)))
            {
                ServerUDPSendPacket(new PkEcho(packet.endPoint.Address), packet.endPoint);
            }
        }
    }
}