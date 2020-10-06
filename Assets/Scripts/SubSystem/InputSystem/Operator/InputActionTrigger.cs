using UnityEngine;

namespace GameSystem.Operator
{
    /// <summary>
    /// 用 Unity Event 输入数据，触发输入系统的 Action。可能在 UI 交互时才用得到吧。
    /// </summary>
    [AddComponentMenu("[InputSystem]/Operator/InputActionTrigger")]
    public class InputActionTrigger : MonoBehaviour
    {
#if UNITY_EDITOR
        [MinsHeader("Operator of InputSystem", SummaryType.PreTitleOperator, -1)]
        [MinsHeader("输入 Action 触发器", SummaryType.TitleOrange, 0)]
        [MinsHeader("用 Unity Event 输入数据，触发输入系统的 Action。可能在 UI 交互时才用得到吧。", SummaryType.CommentCenter, 1)]
        [ConditionalShow, SerializeField] private bool useless; //在没有数据的时候让标题正常显示
#endif

        //Input
        public void Move(Vector2 val) { InputSystem.Move(val); }
    }
}