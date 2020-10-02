using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Linker
    {
        [AddComponentMenu("Linker/InputSystem/InputGetter")]
        public class InputGetter : MonoBehaviour
        {
            [MinsHeader("Linker of InputSystem", SummaryType.PreTitleLinker, -1)]
            [MinsHeader("Input Getter", SummaryType.TitleBlue, 0)]
            [MinsHeader("输入系统获取输入按钮事件", SummaryType.CommentCenter, 1)]

            //Data
            [MinsHeader("Data", SummaryType.Header, 2)]
            [Label]
            public InputSystem.InputKey key;
            [Label]
            public bool anyKey;

            private void Update()
            {
                if (anyKey ? Input.anyKey : InputSystem.GetKey(key)) keyOutput?.Invoke();
                if (anyKey ? Input.anyKeyDown : InputSystem.GetKeyDown(key)) keyDownOutput?.Invoke();
                if (InputSystem.GetKeyUp(key)) keyUpOutput?.Invoke();
            }

            //Output
            [MinsHeader("Output", SummaryType.Header, 3)]
            public SimpleEvent keyOutput;
            public SimpleEvent keyDownOutput;
            public SimpleEvent keyUpOutput;
        }
    }
}
