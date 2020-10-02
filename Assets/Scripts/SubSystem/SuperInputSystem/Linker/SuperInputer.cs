using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

namespace GameSystem
{
    namespace Linker
    {
        /// <summary>
        /// 玩家输入器
        /// </summary>
        [AddComponentMenu("Linker/SuperInputSystem/SuperInputer")]
        public class SuperInputer : MonoBehaviour
        {
            [MinsHeader("Linker of SuperInputSystem", SummaryType.PreTitleLinker, -1)]
            [MinsHeader("玩家输入器", SummaryType.TitleBlue, 0)]

            //Data
            [MinsHeader("Data", SummaryType.Header, 2)]
            [Label("输入动作", true)]
            public SuperInputSystem.InputActions inputAction;

            private void Start()
            {
                switch (inputAction)
                {
                    case SuperInputSystem.InputActions.point: SuperInputSystem.point += vec2Input.Invoke; break;
                    case SuperInputSystem.InputActions.drag: SuperInputSystem.drag += vec2Input.Invoke; break;
                    case SuperInputSystem.InputActions.slide: SuperInputSystem.slide += floatInput.Invoke; break;
                }
            }

            //Output
            [MinsHeader("Output", SummaryType.Header, 3)]
            [ConditionalShow("inputAction",
                SuperInputSystem.InputActions.point,
                SuperInputSystem.InputActions.drag)]
            public Vec2Event vec2Input;
            [ConditionalShow("inputAction", SuperInputSystem.InputActions.slide)]
            public FloatEvent floatInput;
        }
    }
}