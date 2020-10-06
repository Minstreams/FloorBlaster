using UnityEngine;

namespace GameSystem.Savable
{
    /// <summary>
    /// 存储个性化信息
    /// </summary>
    [CreateAssetMenu(fileName = "PersonalizationData", menuName = "Savable/PersonalizationData")]
    public class PersonalizationData : SavableObject
    {
        [MinsHeader("SavableObject of PersonalizationSystem", SummaryType.PreTitleSavable, -1)]
        [MinsHeader("PersonalizationData", SummaryType.TitleGreen, 0)]
        [MinsHeader("存储个性化信息", SummaryType.CommentCenter, 1)]

        [MinsHeader("Data", SummaryType.Header), Space(16)]
        [Label("本地玩家信息")]
        public PersonalizationSystem.PlayerInfo localPlayerInfo;
        [Label("本地房间信息")]
        public PersonalizationSystem.RoomInfo localRoomInfo;

        //APPLY the data to game
        public override void ApplyData()
        {
            PersonalizationSystem.Setting.localPlayerInfo = localPlayerInfo;
            PersonalizationSystem.Setting.localRoomInfo = localRoomInfo;
        }

        //Collect and UPDATE data
        public override void UpdateData()
        {
            localPlayerInfo = PersonalizationSystem.LocalPlayerInfo;
            localRoomInfo = PersonalizationSystem.LocalRoomInfo;
        }
    }
}