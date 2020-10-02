
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Linker
    {
        [AddComponentMenu("Linker/LerpColorLinker")]
        public class LerpColorLinker : MonoBehaviour
        {
            [MinsHeader("Lerp Color Linker", SummaryType.TitleCyan, 0)]

            //Data
            [MinsHeader("Data", SummaryType.Header, 2)]
            [Label]
            public Color colorA = Color.white;
            [Label]
            public Color colorB = Color.white;
            [Label]
            public AnimationCurve remapCurve = AnimationCurve.Linear(0, 0, 1, 1);
            [Label]
            public bool clamp = false;

            //Output
            [MinsHeader("Output", SummaryType.Header, 3)]
            public ColorEvent output;

            //Input
            public void Invoke(float input)
            {
                float t = remapCurve.Evaluate(clamp ? Mathf.Clamp01(input) : input);
                output?.Invoke(Color.Lerp(colorA, colorB, t));
            }
        }
    }
}
