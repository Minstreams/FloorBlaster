using UnityEngine;

namespace GameSystem.Linker
{
    [AddComponentMenu("|Linker/LerpVec3Linker")]
    public class LerpVec3Linker : MonoBehaviour
    {
        [MinsHeader("Lerp Vec3 Linker", SummaryType.TitleCyan, 0)]

        //Data
        [MinsHeader("Data", SummaryType.Header, 2)]
        [Label]
        public AnimationCurve remapXCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [Label]
        public AnimationCurve remapYCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [Label]
        public AnimationCurve remapZCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [Label]
        public bool clamp = false;

        //Output
        [MinsHeader("Output", SummaryType.Header, 3)]
        public Vec3Event output;

        //Input
        public void Invoke(float input)
        {
            float t = clamp ? Mathf.Clamp01(input) : input;
            output?.Invoke(new Vector3(remapXCurve.Evaluate(t), remapYCurve.Evaluate(t), remapZCurve.Evaluate(t)));
        }
    }
}