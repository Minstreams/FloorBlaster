using UnityEngine;
using System.Net;
using GameSystem.Networking.Packet;

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
        protected static PacketBase StringToPacket(string str) => NetworkSystem.StringToPacket(str);
        protected static string PacketToString(PacketBase pkt) => NetworkSystem.PacketToString(pkt);
        protected static void ClientSendPacket(PacketBase pkt) => NetworkSystem.ClientSendPacket(pkt);
        protected static void ClientUDPSendPacket(PacketBase pkt, IPEndPoint endPoint) => NetworkSystem.ClientUDPSendPacket(pkt, endPoint);
        protected static void ServerBoardcastPacket(PacketBase pkt) => NetworkSystem.ServerBoardcastPacket(pkt);
        protected static void ServerUDPSendPacket(PacketBase pkt, IPEndPoint endPoint) => NetworkSystem.ServerUDPSendPacket(pkt, endPoint);
        protected static void ServerUDPBoardcastPacket(PacketBase pkt) => NetworkSystem.ServerUDPBoardcastPacket(pkt);
    }
}