using System.Collections;
using System.Collections.Generic;
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
        public int serverUDPPort = 7856;
        [Label]
        public int minClientPort = 12306;
        [Label]
        public int maxClientPort = 17851;
        [Label]
        public string serverIP = "127.0.0.1";
        [Label]
        public string localIP = "127.0.0.1";

        [MinsHeader("客户端广播配置（寻找服务器）", SummaryType.Header), Space(16)]
        [Label("广播间隔")]
        public int udpBoardcastInterval = 50;
        [Label("广播刷新间隔")]
        public int udpBoardcastRefreshInterval = 5000;
    }
}