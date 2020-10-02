using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Operator
    {
        [AddComponentMenu("Operator/CanvasMaterialFloatSetter")]
        public class CanvasMaterialFloatSetter : MonoBehaviour
        {
            [MinsHeader("Canvas Material Float Setter", SummaryType.TitleYellow, 0)]

            //Data
            [MinsHeader("Data", SummaryType.Header, 2)]
            [Label]
            public float target = 0.3f;
            [Label]
            public float time = 0.5f;
            [Label]
            public string paramName = "_EmissionFactor";

            [Label("Canvas Renderer")]
            public UnityEngine.UI.Image canvasRenderer;

            [Label(true)]
            public bool setOnStart = true;
            [Label(true)]
            public bool setOnEnable = false;

            //Inner code here
            private void Start()
            {
                if (setOnStart) Set();
            }
            private void OnEnable()
            {
                if (setOnEnable) Set();
            }

            //Input
            [ContextMenu("Set")]
            public void Set()
            {
                Set(target);
            }
            public void Set(float target)
            {
                canvasRenderer.material.SetFloat(paramName, target);
            }

        }
    }
}