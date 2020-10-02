using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Linker
    {
        [AddComponentMenu("Linker/AudioSystem/AudioConfigLinker")]
        public class AudioConfigLinker : MonoBehaviour
        {
#if UNITY_EDITOR
            [MinsHeader("Linker of AudioSystem", SummaryType.PreTitleLinker, -1)]
            [MinsHeader("Audio Config Linker", SummaryType.TitleBlue, 0)]
            [ConditionalShow, SerializeField] private bool useless;
#endif

            //Data
            [MinsHeader("Data", SummaryType.Header, 2)]
            [Label]
            public AudioSystem.AudioConfig data;

            //Output
            [MinsHeader("Output", SummaryType.Header, 3)]
            public AudioConfigEvent output;

            //Input
            [ContextMenu("Invoke")]
            public virtual void Invoke()
            {
                output?.Invoke(data);
            }
            public void SetLoop(bool loop)
            {
                data.loop = loop;
            }
            public void SetPitch(float pitch)
            {
                data.pitch = pitch;
            }
            public void SetVolume(float volume)
            {
                data.volume = volume;
            }
        }
    }
}
