using UnityEngine;

namespace GameSystem.Linker
{
    /// <summary>
    /// 获取键盘鼠标的输入，并以 Unity Event 的形式输出。这应该只作为临时调试使用，或用来绑定 Input Action。
    /// </summary>
    [AddComponentMenu("[InputSystem]/Linker/InputKeyGetter")]
    public class InputKeyGetter : MonoBehaviour
    {
#if UNITY_EDITOR
        [MinsHeader("Linker of InputSystem", SummaryType.PreTitleLinker, -1)]
        [MinsHeader("键盘按键获取器", SummaryType.TitleBlue, 0)]
        [MinsHeader("获取键盘鼠标的输入，并以 Unity Event 的形式输出。这应该只作为临时调试使用，或用来绑定 Input Action。", SummaryType.CommentCenter, 1)]
        [ConditionalShow, SerializeField] private bool useless; //在没有数据的时候让标题正常显示
#endif

        //Data
        [MinsHeader("输入", SummaryType.Header, 2)]
        [Label]
        public bool anyKey;
        [ConditionalShow("anyKey", false, AlwaysShow = true)]
        public InputSystem.InputKey key;

        private void Update()
        {
            if (anyKey ? Input.anyKey : InputSystem.GetKey(key)) keyOutput?.Invoke();
            if (anyKey ? Input.anyKeyDown : InputSystem.GetKeyDown(key)) keyDownOutput?.Invoke();
            if ((!anyKey) && InputSystem.GetKeyUp(key)) keyUpOutput?.Invoke();
        }

        //Output
        [MinsHeader("Output", SummaryType.Header, 3)]
        public SimpleEvent keyOutput;
        public SimpleEvent keyDownOutput;
        [ConditionalShow("anyKey", false, AlwaysShow = true)]
        public SimpleEvent keyUpOutput;
    }
}