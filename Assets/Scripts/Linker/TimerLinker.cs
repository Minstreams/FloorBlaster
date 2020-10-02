using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Linker
    {
        [AddComponentMenu("Linker/TimerLinker")]
        public class TimerLinker : MonoBehaviour
        {
            [MinsHeader("Timer Linker", SummaryType.TitleCyan, 0)]

            //Data
            [MinsHeader("Data", SummaryType.Header, 2)]
            [Label]
            [Tooltip("time must be between 0 and 1")]
            public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
            [Label("End Time")]
            public float time = 1;
            private IEnumerator invoke(float time)
            {
                float timer = 0;
                while (timer < 1)
                {
                    yield return 0;
                    output?.Invoke(curve.Evaluate(timer));
                    timer += Time.deltaTime / time;
                }
                output?.Invoke(curve.Evaluate(1));
            }

            //Output
            [MinsHeader("Output", SummaryType.Header, 3)]
            public FloatEvent output;


            //Input
            [ContextMenu("Invoke")]
            public void Invoke()
            {
                StopAllCoroutines();
                StartCoroutine(invoke(time));
            }
            public void Invoke(float time)
            {
                StartCoroutine(invoke(time));
            }
        }
    }
}