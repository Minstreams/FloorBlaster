using UnityEngine;

namespace GameSystem.Setting
{
    [CreateAssetMenu(fileName = "NetworkSystemSetting", menuName = "系统配置文件/NetworkSystemSetting")]
    public class NetworkSystemSetting : ScriptableObject
    {
        [MinsHeader("NetworkSystem Setting", SummaryType.Title, -2)]
        [MinsHeader("网络系统。用于多人联机和服务器服务。", SummaryType.CommentCenter, -1)]

        [MinsHeader("PORT", SummaryType.Header), Space(16)]
        [Label]
        public int serverTCPPort = 7858;
        [Label]
        public int serverUDPPort = 7857;
        [Label]
        public int clientUDPPort = 7856;
        [Label]
        public int minClientPort = 12306;
        [Label]
        public int maxClientPort = 17851;

        [MinsHeader("服务器大厅广播配置", SummaryType.Header), Space(16)]
        [LabelRange("广播间隔", 0.0001f, 10f)]
        public float udpBoardcastInterval = 1f;

        [MinsHeader("游戏中同步设置", SummaryType.Header), Space(16)]
        /// <summary>
        /// 在这个单位时间内完成一次lerp
        /// </summary>
        [LabelRange(0.05f, 1)]
        public float lerpTime = 0.385f;
        [Label]
        public float inputSendInterval = 0.1f;
        /// <summary>
        /// 变化超过阈值立即同步
        /// </summary>
        [Label]
        public float inputSendMoveThreadhold = 0.5f;
    }
}