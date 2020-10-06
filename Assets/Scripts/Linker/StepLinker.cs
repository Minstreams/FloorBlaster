using UnityEngine;

namespace GameSystem.Linker
{
    [AddComponentMenu("|Linker/StepLinker")]
    public class StepLinker : MonoBehaviour
    {
        [MinsHeader("Step Linker", SummaryType.TitleCyan, 0)]

        //Data
        [MinsHeader("Data", SummaryType.Header, 2)]
        [Label]
        public float stepThreadhold = 1;

        float stepper = 0;

        //Output
        [MinsHeader("Output", SummaryType.Header, 3)]
        public SimpleEvent output;

        //Input
        [ContextMenu("Invoke")]
        public void Invoke()
        {
            output?.Invoke();
        }
        public void StepForward(float input)
        {
            stepper += input;
            while (stepper > stepThreadhold)
            {
                stepper -= stepThreadhold;
                output?.Invoke();
            }
        }
        public void ResetStepper()
        {
            stepper = 0;
        }

    }
}