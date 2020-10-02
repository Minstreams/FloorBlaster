using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Linker
    {
        [AddComponentMenu("Linker/LoopLinker")]
        public class LoopLinker : MonoBehaviour
        {
            //Private Here
            private int index = 0;

            [MinsHeader("Loop Linker", SummaryType.TitleCyan, 0)]

            //Output
            [MinsHeader("Output", SummaryType.Header, 3)]
            public SimpleEvent[] outputs;

            //Input
            [ContextMenu("Invoke")]
            public void Invoke()
            {
                outputs[index++]?.Invoke();
                if (index > outputs.Length) index = 0;
            }
            public void ResetIndex()
            {
                index = 0;
            }

        }
    }
}