using UnityEngine;

namespace GameSystem.Linker
{
    [AddComponentMenu("[AudioSystem]/Linker/AudioConfigSplitter")]
    public class AudioConfigSplitter : MonoBehaviour
    {
        [MinsHeader("Linker of AudioSystem", SummaryType.PreTitleLinker, -1)]
        [MinsHeader("Audio Config Splitter", SummaryType.TitleBlue, 0)]

        //Output
        [MinsHeader("Output", SummaryType.Header, 3)]
        public BoolEvent loopOutput;
        public FloatEvent pitchOutput;
        public FloatEvent volumeOutput;

        //Input
        public void Invoke(AudioSystem.AudioConfig data)
        {
            loopOutput?.Invoke(data.loop);
            pitchOutput?.Invoke(data.pitch);
            volumeOutput?.Invoke(data.volume);
        }
    }
}