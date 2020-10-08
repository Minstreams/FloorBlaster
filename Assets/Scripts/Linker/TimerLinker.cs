using System.Collections;
using UnityEngine;

namespace GameSystem.Linker
{
    [AddComponentMenu("|Linker/TimerLinker")]
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
        IEnumerator invoke(float time)
        {
            float timer = 0;
            while (timer < 1)
            {
                yield return 0;
                output?.Invoke(curve.Evaluate(timer));
                timer += Time.deltaTime / time;
            }
            output?.Invoke(curve.Evaluate(1));
            onFinish?.Invoke();
        }

        //Output
        [MinsHeader("Output", SummaryType.Header, 3)]
        public FloatEvent output;
        public SimpleEvent onFinish;


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