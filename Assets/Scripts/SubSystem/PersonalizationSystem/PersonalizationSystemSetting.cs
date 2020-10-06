using UnityEngine;

namespace GameSystem.Setting
{
    [CreateAssetMenu(fileName = "PersonalizationSystemSetting", menuName = "系统配置文件/PersonalizationSystemSetting")]
    public class PersonalizationSystemSetting : ScriptableObject
    {
        [MinsHeader("PersonalizationSystem Setting", SummaryType.Title, -2)]
        [MinsHeader("个性化系统，用于定义玩家个性化信息", SummaryType.CommentCenter, -1)]

        [MinsHeader("Data", SummaryType.Header), Space(16)]
        [Label("本地玩家信息")]
        public PersonalizationSystem.PlayerInfo localPlayerInfo;
        [Label("本地房间信息")]
        public PersonalizationSystem.RoomInfo localRoomInfo;
    }
}