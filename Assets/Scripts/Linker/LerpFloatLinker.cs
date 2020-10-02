using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Linker
    {
        [AddComponentMenu("Linker/LerpFloatLinker")]
        public class LerpFloatLinker : MonoBehaviour
        {
            [MinsHeader("Lerp Float Linker", SummaryType.TitleCyan, 0)]

            //Data
            [MinsHeader("Data", SummaryType.Header, 2)]
            [Label]
            public AnimationCurve remapCurve = AnimationCurve.Linear(0, 0, 1, 1);
            [Label]
            public bool clamp = false;

            //Output
            [MinsHeader("Output", SummaryType.Header, 3)]
            public FloatEvent output;

            //Input
            public void Invoke(float input)
            {
                output?.Invoke(remapCurve.Evaluate(clamp ? Mathf.Clamp01(input) : input));
            }
        }
    }
}