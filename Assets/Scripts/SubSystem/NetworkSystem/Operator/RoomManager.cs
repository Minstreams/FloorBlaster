using System.Collections;
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
        public string thisID;
        public Transform playerRoot;
        void Start()
        {
            thisID = NetworkSystem.netId;
            playersDatabase.Add(NetworkSystem.netId, PlayerAvater.local);
            if (IsServer)
            {
                roomInfo = PersonalizationSystem.LocalRoomInfo;
                StartCoroutine(BoardcastBrief());
            }
            else
            {
                // 客户端发送信息同步请求
                ClientSendPacket(new RInfo());
                // 将自己的玩家信息发送给服务器
                ClientSendPacket(new CPlayerInfo(PersonalizationSystem.LocalPlayerInfo));
            }
        }

        // 玩家管理 -------------------------------------
        Dictionary<string, PlayerAvater> playersDatabase = new Dictionary<string, PlayerAvater>();
        Queue<PlayerRecordUnit> serverCallAddPlayerQueue = new Queue<PlayerRecordUnit>();
        Queue<string> serverCallDeletePlayerQueue = new Queue<string>();
        [System.Serializable]
        public struct PlayerRecordUnit
        {
            public string id;
            public PersonalizationSystem.PlayerInfo info;
            public Vector3 pos;

            public PlayerRecordUnit(string id, PersonalizationSystem.PlayerInfo info, Vector3 pos)
            {
                this.id = id;
                this.info = info;
                this.pos = pos;
            }
        }
        void ServerCallAddPlayer(string id, PersonalizationSystem.PlayerInfo info, Vector3 pos)
        {
            serverCallAddPlayerQueue.Enqueue(new PlayerRecordUnit(id, info, pos));
        }
        void AddPlayer(string id, PersonalizationSystem.PlayerInfo info, Vector3 pos)
        {
            var g = GameObject.Instantiate(Setting.playerPrefab, pos, Quaternion.identity, playerRoot);
            var avater = g.GetComponent<PlayerAvater>();
            avater.info = info;
            avater.ActivateId(id);
            playersDatabase.Add(avater.netId, avater);
        }
        void ServerCallDeletePlayer(string id)
        {
            serverCallDeletePlayerQueue.Enqueue(id);
        }
        void DeletePlayer(string id)
        {
            if (!playersDatabase.ContainsKey(id)) throw new System.Exception("正在试图删除一个不存在的玩家。ID：" + id);
            Destroy(playersDatabase[id].gameObject);
            playersDatabase.Remove(id);
        }


        // 客户端 ---------------------------------------
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
                }
                else
                {
                    AddPlayer(p.id, p.info, p.pos);
                }
            }
        }

        public Vector3 NewPlayerPos()
        {
            return new Vector3(7, 0.5f, 7);
        }

        // 服务器 TCP -----------------------------------
        [TCPProcess]
        void RInfoProcess(RInfo pkt, Server.Connection connection)
        {
            // 收到客户端的请求，发送房间信息
            connection.Send(PacketToString(new SInfo(roomInfo)));
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
                ServerCallAddPlayer(connection.netId, pkt.info, NewPlayerPos());
            }
        }
        [TCPDisconnectionProcess]
        void OnDisconnection(Server.Connection connection)
        {
            ServerCallDeletePlayer(connection.netId);
        }


        private void Update()
        {
            if (IsServer)
            {
                while (serverCallAddPlayerQueue.Count > 0)
                {
                    var p = serverCallAddPlayerQueue.Dequeue();
                    AddPlayer(p.id, p.info, p.pos);
                    ServerBoardcastPacket(new SPlayerInfo(playersDatabase));
                }
                while (serverCallDeletePlayerQueue.Count > 0)
                {
                    DeletePlayer(serverCallDeletePlayerQueue.Dequeue());
                }
            }
        }

        // 服务器 UDP -----------------------------------
        [UDPProcess]
        void UDPPRocess(UDPPacket packet)
        {
            // 服务器处理Echo
            PacketBase pkt = StringToPacket(packet.message);
            if (pkt.MatchType(typeof(UEcho)))
            {
                ServerUDPSendPacket(new UEcho(packet.endPoint.Address), packet.endPoint);
            }
        }

        IEnumerator BoardcastBrief()
        {
            while (true)
            {
                ServerUDPBoardcastPacket(new URoomBrief(roomInfo.name));
                yield return new WaitForSeconds(Setting.udpBoardcastInterval);
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("LocalIP:" + NetworkSystem.LocalIPAddress);
            GUILayout.Label("ServerIP:" + NetworkSystem.ServerIPAddress);
            GUILayout.Label("Room:" + roomInfo.name);
            GUILayout.Label(thisID);
        }
    }
}