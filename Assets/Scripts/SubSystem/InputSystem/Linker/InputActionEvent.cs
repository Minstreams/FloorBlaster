using UnityEngine;

namespace GameSystem.Linker
{
    /// <summary>
    /// 用 Unity Event 捕获输入系统的 Action。这应该作为输入系统和游戏交互的唯一途径。
    /// </summary>
    [AddComponentMenu("[InputSystem]/Linker/InputActionEvent")]
    public class InputActionEvent : MonoBehaviour
    {
#if UNITY_EDITOR
        [MinsHeader("Linker of InputSystem", SummaryType.PreTitleLinker, -1)]
        [MinsHeader("输入 Action 事件", SummaryType.TitleBlue, 0)]
        [MinsHeader("用 Unity Event 捕获输入系统的 Action。这应该作为输入系统和游戏交互的唯一途径。", SummaryType.CommentCenter, 1)]
        [ConditionalShow, SerializeField] private bool useless; //在没有数据的时候让标题正常显示
#endif
        //Data
        [MinsHeader("输入", SummaryType.Header, 2)]
        [Label("输入Action", true)]
        public InputSystem.InputAction inputAction;

        private void Start()
        {
            switch (inputAction)
            {
                case InputSystem.InputAction.Any: InputSystem._Any += simpleOutput.Invoke; break;
                case InputSystem.InputAction.AnyDown: InputSystem._AnyDown += simpleOutput.Invoke; break;
                case InputSystem.InputAction.Move: InputSystem._Move += vec2Output.Invoke; break;
            }
        }

        //Output
        [MinsHeader("输出", SummaryType.Header, 3)]

        [ConditionalShow("inputAction",
            InputSystem.InputAction.Any,
            InputSystem.InputAction.AnyDown,
            Label = "输出"
        )]
        public SimpleEvent simpleOutput;

        [ConditionalShow(
            Label = "输出"
        )]
        public FloatEvent floatOutput;

        [ConditionalShow("inputAction",
            InputSystem.InputAction.Move,
            Label = "输出"
        )]
        public Vec2Event vec2Output;
    }
}