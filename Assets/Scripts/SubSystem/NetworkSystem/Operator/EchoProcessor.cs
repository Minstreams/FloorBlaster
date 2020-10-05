using GameSystem.Networking;
using GameSystem.Networking.Packet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Operator
    {
        /// <summary>
        /// 用这项技术来获取本地和服务器的IP地址
        /// </summary>
        [AddComponentMenu("[NetworkSystem]/Operator/EchoProcessor")]
        public class EchoProcessor : NetworkObject
        {
#if UNITY_EDITOR
            [MinsHeader("Operator of NetworkSystem", SummaryType.PreTitleOperator, -1)]
            [MinsHeader("IP回声处理器", SummaryType.TitleOrange, 0)]
            [MinsHeader("用这项技术来获取本地和服务器的IP地址", SummaryType.CommentCenter, 1)]
            [ConditionalShow, SerializeField] private bool useless; //在没有数据的时候让标题正常显示
#endif
            [UDPReceive]
            void EchoReceive(UDPPacket packet)
            {
                PacketBase pkt = NetworkSystem.StringToPacket(packet.message);
                if (pkt.MatchType(typeof(PacketIPEcho)))
                {
                    // 客户端收到Echo，确定自己的地址
                    if (NetworkSystem.localIPCheck) return;
                    PacketIPEcho pktEcho = pkt as PacketIPEcho;
                    NetworkSystem.LocalIPAddress = pktEcho.address;
                }
            }
            [UDPProcess]
            void EchoProcess(UDPPacket packet)
            {
                PacketBase pkt = NetworkSystem.StringToPacket(packet.message);
                if (pkt.MatchType(typeof(PacketIPEcho)))
                {
                    // 服务器收到Echo，确定自己的地址
                    if (!NetworkSystem.server.TcpOn)
                    {
                        PacketIPEcho pktEcho = pkt as PacketIPEcho;
                        NetworkSystem.LocalIPAddress = pktEcho.address;
                        NetworkSystem.server.TurnOnTCP();
                    }
                    NetworkSystem.server.UDPSend(NetworkSystem.PacketToString(new PacketIPEcho(packet.endPoint.Address)), packet.endPoint);
                }
            }
        }
    }
}