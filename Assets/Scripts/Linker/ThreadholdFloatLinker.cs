using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Linker
    {
        [AddComponentMenu("|Linker/ThreadholdFloatLinker")]
        public class ThreadholdFloatLinker : MonoBehaviour
        {
            [MinsHeader("Threadhold Float Linker", SummaryType.TitleCyan, 0)]

            //Data
            [MinsHeader("Data", SummaryType.Header, 2)]
            [Label]
            public float threadhold = 0.5f;

            private float value = 0;

            //Output
            [MinsHeader("Output", SummaryType.Header, 3)]
            public SimpleEvent onOverThreadhold;
            public SimpleEvent onBelowThreadhold;


            //Input
            public void Invoke(float val)
            {
                if (value < threadhold && val >= threadhold) onOverThreadhold?.Invoke();
                if (value < threadhold && val <= threadhold) onBelowThreadhold?.Invoke();
                value = val;
            }
            public void SetThreadhold(float val)
            {
                threadhold = val;
            }
        }
    }
}
