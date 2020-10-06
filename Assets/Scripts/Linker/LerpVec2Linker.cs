using UnityEngine;

namespace GameSystem.Linker
{
    [AddComponentMenu("|Linker/LerpVec2Linker")]
    public class LerpVec2Linker : MonoBehaviour
    {
        [MinsHeader("Lerp Vec2 Linker", SummaryType.TitleCyan, 0)]

        //Data
        [MinsHeader("Data", SummaryType.Header, 2)]
        [Label]
        public AnimationCurve remapXCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [Label]
        public AnimationCurve remapYCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [Label]
        public bool clamp = false;

        //Output
        [MinsHeader("Output", SummaryType.Header, 3)]
        public Vec2Event output;

        //Input
        public void Invoke(float input)
        {
            float t = clamp ? Mathf.Clamp01(input) : input;
            output?.Invoke(new Vector2(remapXCurve.Evaluate(t), remapYCurve.Evaluate(t)));
        }
    }
}