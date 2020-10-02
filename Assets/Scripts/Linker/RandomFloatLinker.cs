using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Linker
    {
        [AddComponentMenu("Linker/RandomFloatLinker")]
        public class RandomFloatLinker : MonoBehaviour
        {
            [MinsHeader("Random Float Linker", SummaryType.TitleCyan, 0)]

            //Data
            [MinsHeader("Data", SummaryType.Header, 2)]
            [Label]
            public Vector2 range = Vector2.up;

            //Output
            [MinsHeader("Output", SummaryType.Header, 3)]
            public FloatEvent output;

            //Input
            public void Invoke(float input)
            {
                output?.Invoke(Random.Range(range.x, range.y));
            }
            public void SetRange(Vector2 range)
            {
                this.range = range;
            }
        }
    }
}
