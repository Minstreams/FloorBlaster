using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Operator
    {
        [AddComponentMenu("Operator/DebugPrinter")]
        public class DebugPrinter : MonoBehaviour
        {

#if UNITY_EDITOR
            [MinsHeader("调试器节点", SummaryType.TitleYellow, 0)]
            [MinsHeader("调用 Print 方法，可以在控制台输出各种类型的数据", SummaryType.CommentCenter, 1)]
            [ConditionalShow, SerializeField] private bool useless;
#endif

            //Input
            public void Print(int val) { Debug.Log(val); }
            public void Print(float val) { Debug.Log(val); }
            public void Print(string val) { Debug.Log(val); }
            public void Print(Vector2 val) { Debug.Log(val); }
            public void Print(Vector3 val) { Debug.Log(val); }
            public void Print(Color val) { Debug.Log(val); }
        }
    }
}