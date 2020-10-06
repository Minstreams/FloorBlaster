using UnityEngine;


namespace GameSystem.Savable
{
    [CreateAssetMenu(fileName = "InputSolutionData", menuName = "Savable/InputSolutionData")]
    public class InputSolutionData : SavableObject
    {
        /// <summary>
        /// 主要按键
        /// </summary>
        [MinsHeader("SavableObject of InputSystem", SummaryType.PreTitleSavable, -3)]
        [MinsHeader("Input Solution Data", SummaryType.TitleGreen, -2)]
        [MinsHeader("用来存储用户的按键设置", SummaryType.CommentCenter, -1)]

        [MinsHeader("KeyBoard", SummaryType.SubTitle)]
        [MinsHeader("主要按键", SummaryType.Header, 1), Space(16)]
        public InputKeyMap MainKeys;
        /// <summary>
        /// 次要按键
        /// </summary>
        [MinsHeader("次要按键", SummaryType.Header), Space(16)]
        public InputKeyMap SideKeys;

        public override void ApplyData()
        {
            InputSystem.Setting.MainKeys = MainKeys;
            InputSystem.Setting.SideKeys = SideKeys;
        }

        public override void UpdateData()
        {
            MainKeys = InputSystem.Setting.MainKeys;
            SideKeys = InputSystem.Setting.SideKeys;
        }
    }

}