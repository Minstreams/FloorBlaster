using System.Collections.Generic;
using UnityEngine;
using GameSystem.Networking;
using GameSystem.Setting;

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
        [Label("房间信息")]
        public PersonalizationSystem.RoomInfo roomInfo;
        public Transform playerRoot;

        float timerTargetOffset = 0;
        float timerOffset { get => NetworkSystem.timerOffset; set => NetworkSystem.timerOffset = value; }
        float latency { get => NetworkSystem.latency; set => NetworkSystem.latency = value; }
        Dictionary<string, PlayerAvater> playersDatabase => GameplaySystem.playersDatabase;


        void Start()
        {
            timer = 0;
            GameplaySystem.playerRoot = playerRoot;

            if (IsServer)
            {
                roomInfo = PersonalizationSystem.LocalRoomInfo;
                ServerUDPSendPacket(new HServerReg(LocalIPAddress.ToString()), NetworkSystem.HelperEndPoint);
            }
            else
            {
                // 请求同步时间
                ClientSendPacket(new RTimer());
                // 客户端发送信息同步请求
                ClientSendPacket(new RInfo());
                // 将自己的玩家信息发送给服务器
                ClientSendPacket(new CPlayerInfo(PersonalizationSystem.LocalPlayerInfo));
            }
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timerTargetOffset - timerOffset > 1) timerOffset = timerTargetOffset;
            else timerOffset = Mathf.Lerp(timerOffset, timerTargetOffset, 0.1f);
        }


        // 客户端 ---------------------------------------
        [TCPReceive]
        void STimerReceive(STimer pkt)
        {
            timerTargetOffset = pkt.t - (pkt.tSent + timer) * 0.5f;
            latency = (timer - pkt.tSent) * 0.5f;
        }
        [TCPReceive]
        void SInfoReceive(SInfo pkt)
        {
            // 收到服务器发来的房间信息
            roomInfo = pkt.info;
        }
        [TCPReceive]
        void SPlayerInfoReceive(SPlayerInfo pkt)
        {
            if (IsServer) return;
            foreach (var p in pkt.records)
            {
                if (playersDatabase.ContainsKey(p.id))
                {
                    playersDatabase[p.id].info = p.info;
                    playersDatabase[p.id].targetPosition = p.pos;
                }
                else
                {
                    GameplaySystem.AddPlayer(p.id, p.info, p.pos);
                }
            }
        }



        // 服务器 TCP -----------------------------------
        [TCPProcess]
        void RTimerProcess(RTimer pkt, Server.Connection connection)
        {
            connection.Send(new STimer(pkt.t));
        }
        [TCPProcess]
        void RInfoProcess(RInfo pkt, Server.Connection connection)
        {
            // 收到客户端的请求，发送房间信息
            connection.Send(new SInfo(roomInfo));
        }
        [TCPProcess]
        void CPlayerInfoProcess(CPlayerInfo pkt, Server.Connection connection)
        {
            // 收到客户端发来的玩家信息
            if (playersDatabase.ContainsKey(connection.netId))
            {
                // 更新数据
                playersDatabase[connection.netId].info = pkt.info;
            }
            else
            {
                // 添加数据
                CallMainThread(() =>
                {
                    GameplaySystem.AddPlayer(connection.netId, pkt.info);
                    ServerBoardcastPacket(new SPlayerInfo());
                });
            }
        }
        [TCPDisconnectionProcess]
        void OnDisconnection(Server.Connection connection)
        {
            CallMainThread(() =>
            {
                GameplaySystem.DeletePlayer(connection.netId);
            });
        }


        // 服务器 UDP -----------------------------------
        [UDPProcess]
        void UDPPRocess(UDPPacket packet)
        {
            // 服务器处理Echo
            PacketBase pkt = StringToPacket(packet.message);
            if (pkt.MatchType(typeof(UHello)))
            {
                var hello = pkt as UHello;
                // 对暗号！
                if (hello.hello != NetworkSystem.clientHello) return;

                ServerUDPSendPacket(new URoomBrief(roomInfo.name), packet.endPoint);
            }
        }


        private void OnGUI()
        {
            if (GUILayout.Button("Disconnect")) NetworkSystem.client.StopTCPConnecting();
            GUILayout.Label("LocalIP:" + NetworkSystem.LocalIPAddress);
            GUILayout.Label("ServerIP:" + NetworkSystem.ServerIPAddress);
            GUILayout.Label("Room:" + roomInfo.name);
            GUILayout.Label("Timer:" + timer);
            GUILayout.Label("TimerOffset:" + timerOffset);
            GUILayout.Label("TimerTargetOffset:" + timerTargetOffset);
            GUILayout.Label("Latency:" + latency);
            GUILayout.Label("Id: " + NetworkSystem.netId);
        }
    }
}