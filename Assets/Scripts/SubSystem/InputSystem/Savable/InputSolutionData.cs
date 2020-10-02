using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameSystem
{
    namespace Savable
    {
        [CreateAssetMenu(fileName = "InputSolutionData", menuName = "Savable/InputSolutionData")]
        public class InputSolutionData : SavableObject
        {
            [MinsHeader("SavableObject of InputSystem", SummaryType.PreTitleSavable, -1)]
            [MinsHeader("Input Solution Data", SummaryType.TitleGreen, 0)]
            [MinsHeader("用来存储用户的按键设置", SummaryType.CommentCenter, 1)]
            [MinsHeader("所有输入按键种类", SummaryType.Header, 2)]
            [Tooltip("asdd")]
            public InputKeyMap Keys;

            public override void ApplyData()
            {
                InputSystem.Setting.Keys = Keys;
            }

            public override void UpdateData()
            {
                Keys = InputSystem.Setting.Keys;
            }
        }

    }
}
