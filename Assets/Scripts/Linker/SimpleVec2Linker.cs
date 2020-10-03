using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Linker
    {
        [AddComponentMenu("|Linker/SimpleVec2Linker")]
        public class SimpleVec2Linker : MonoBehaviour
        {
            [MinsHeader("Simple Vec2 Linker", SummaryType.TitleCyan, 0)]

            //Data
            [MinsHeader("Data", SummaryType.Header, 2)]
            [Label]
            public Vector2 data;
            [Label(true)]
            public bool invokeOnStart;

            private void Start()
            {
                if (invokeOnStart) Invoke();
            }

            //Output
            [MinsHeader("Output", SummaryType.Header, 3)]
            public Vec2Event output;

            //Input
            [ContextMenu("Invoke")]
            public void Invoke()
            {
                output?.Invoke(data);
            }
            public void SetX(float x)
            {
                data.x = x;
            }
            public void SetY(float y)
            {
                data.y = y;
            }
        }
    }
}