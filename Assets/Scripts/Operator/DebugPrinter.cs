using UnityEngine;

namespace GameSystem.Operator
{
    [AddComponentMenu("|Operator/DebugPrinter")]
    public class DebugPrinter : MonoBehaviour
    {

#if UNITY_EDITOR
        [MinsHeader("调试器节点", SummaryType.TitleYellow, 0)]
        [MinsHeader("调用 Print 方法，可以在控制台输出各种类型的数据", SummaryType.CommentCenter, 1)]
        [ConditionalShow, SerializeField] bool useless;
#endif
        [ConditionalShow(AlwaysShow = true, Label = "输出")]
        public string debugStr;

        //Input
        public void Print(string val) { Debug.Log(val); debugStr = val.ToString(); }
        public void Print(int val) { Print(val.ToString()); }
        public void Print(float val) { Print(val.ToString()); }
        public void Print(Vector2 val) { Print(val.ToString()); }
        public void Print(Vector3 val) { Print(val.ToString()); }
        public void Print(Color val) { Print(val.ToString()); }

#if UNITY_EDITOR
        string preStr = "";
        int currentIndex = 0;
        static int count = 0;
        float singleLineHeight = UnityEditor.EditorGUIUtility.singleLineHeight;
        GUIStyle style;
        private void Start()
        {
            currentIndex = count;
            count++;
            var g = transform;
            while (g != null)
            {
                preStr = "/" + g.name + preStr;
                g = g.parent;
            }
            preStr = gameObject.scene.name + preStr;
            style = new GUIStyle("box");
            singleLineHeight = style.lineHeight + style.margin.vertical + 2;
        }
        void OnGUI()
        {
            GUILayout.Space(singleLineHeight * currentIndex + 2);
            GUILayout.BeginHorizontal();
            GUILayout.Space(2);
            GUILayout.Label("[" + preStr + "]" + debugStr, "box");
            GUILayout.EndHorizontal();
        }
#endif
    }
}