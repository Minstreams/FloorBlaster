using UnityEngine;

namespace GameSystem.Operator
{
    /// <summary>
    /// simply keep rotating around an axis
    /// </summary>
    [AddComponentMenu("|Operator/AutoRotater")]
    public class AutoRotater : MonoBehaviour
    {
        [MinsHeader("Auto Rotater", SummaryType.TitleYellow, 0)]
        [MinsHeader("Simply keep rotating around an axis", SummaryType.CommentCenter, 1)]

        //Data
        [MinsHeader("Data", SummaryType.Header, 2)]
        [Label]
        public Vector3 axis = Vector3.up;
        [Label]
        public float speed = 1;
        [LabelRange(0, 1)]
        public float factor = 1;
        [Label]
        public Space relatedSpace;

        void Update()
        {
            transform.Rotate(axis, speed * factor * Time.deltaTime, relatedSpace);
        }

        //Input
        public void SetFactor(float factor)
        {
            this.factor = factor;
        }
    }
}