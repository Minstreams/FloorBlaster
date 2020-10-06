using UnityEngine;
using GameSystem.Savable;

namespace GameSystem.Setting
{
    [CreateAssetMenu(fileName = "TheMatrixSetting", menuName = "系统配置文件/TheMatrixSetting")]
    public class TheMatrixSetting : ScriptableObject
    {
        [MinsHeader("TheMatrix Setting", SummaryType.Title, -2)]
        [MinsHeader("母体，游戏流程控制与消息处理", SummaryType.CommentCenter, -1)]

        [MinsHeader("游戏场景表列", SummaryType.Header), Space(16)]
        public GameSceneMap gameSceneMap;
        [MinsHeader("所有要自动保存的数据", SummaryType.Header), Space(16)]
        [Label("Savable", true)]
        public SavableObject[] dataAutoSave;
    }
}