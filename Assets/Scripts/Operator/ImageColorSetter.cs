using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameSystem
{
    namespace Operator
    {
        [AddComponentMenu("Operator/ImageColorSetter")]
        public class ImageColorSetter : MonoBehaviour
        {
#if UNITY_EDITOR
            [MinsHeader("Image Color Setter", SummaryType.TitleYellow, 0)]
            [ConditionalShow, SerializeField] private bool useless;
#endif

            //Data
            [MinsHeader("Data", SummaryType.Header, 2)]
            [Label]
            public Image target;

            //Input
            public void Set(Color color)
            {
                target.color = color;
            }
        }
    }
}
