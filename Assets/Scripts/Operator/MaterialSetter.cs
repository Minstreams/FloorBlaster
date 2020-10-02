using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Operator
    {
        [AddComponentMenu("Operator/MaterialSetter")]
        public class MaterialSetter : MonoBehaviour
        {
#if UNITY_EDITOR
            [MinsHeader("Material Setter", SummaryType.TitleYellow, 0)]
            [ConditionalShow, SerializeField] private bool useless;
#endif

            //Data
            [MinsHeader("Data", SummaryType.Header, 2)]
            [Label]
            public int index;

            [System.Serializable]
            public struct MaterialPair
            {
                public Renderer renderer;
                public int index;
                public Material mat;
            }
            [System.Serializable]
            public struct MaterialSettingPreset
            {
                [Label("Mat Pair")]
                public MaterialPair[] materialPairs;
            }
            [Label("Present")]
            public MaterialSettingPreset[] presets;

            [Label]
            public bool setOnEnable;

            private void OnEnable()
            {
                if (setOnEnable) Set();
            }


            //Input
            [ContextMenu("Set")]
            public void Set()
            {
                Set(index);
            }
            public void Set(int index)
            {
                foreach (MaterialPair mp in presets[index].materialPairs)
                {
                    Material[] ms = mp.renderer.sharedMaterials;
                    ms[mp.index] = mp.mat;
                    mp.renderer.sharedMaterials = ms;
                }
            }
        }
    }
}