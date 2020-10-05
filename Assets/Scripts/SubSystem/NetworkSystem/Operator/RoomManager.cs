using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Networking.Packet;

namespace GameSystem
{
    namespace Operator
    {
        /// <summary>
        /// 用来管理房间界面的网络数据传输
        /// </summary>
        [AddComponentMenu("[NetworkSystem]/Operator/RoomManager")]
        public class RoomManager : MonoBehaviour
        {
#if UNITY_EDITOR
            [MinsHeader("Operator of NetworkSystem", SummaryType.PreTitleOperator, -1)]
            [MinsHeader("房间管理器", SummaryType.TitleOrange, 0)]
            [MinsHeader("用来管理房间界面的网络数据传输", SummaryType.CommentCenter, 1)]
            [ConditionalShow, SerializeField] private bool useless; //在没有数据的时候让标题正常显示
#endif

            public static string currentRoomName;
            private void Start()
            {
                if (NetworkSystem.isHost)
                {
                    StartCoroutine(BoardcastInfo());
                }
            }

            private IEnumerator BoardcastInfo()
            {
                while (true)
                {
                    NetworkSystem.server.UDPBoardcast(NetworkSystem.PacketToString(new PacketRoomInfo(currentRoomName)));
                    yield return new WaitForSeconds(NetworkSystem.Setting.udpBoardcastInterval);
                }
            }
        }
    }
}