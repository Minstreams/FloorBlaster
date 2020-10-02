using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameSystem
{
    namespace Setting
    {
        [CreateAssetMenu(fileName = "InputSystemSetting", menuName = "系统配置文件/InputSystemSetting")]
        public class InputSystemSetting : ScriptableObject
        {
            [MinsHeader("InputSystem Setting", SummaryType.Title, -2)]
            [MinsHeader("封装输入的系统", SummaryType.CommentCenter, -1)]

            [MinsHeader("所有输入按键种类", SummaryType.Header), Space(16)]
            public InputKeyMap Keys;
        }
    }
}
