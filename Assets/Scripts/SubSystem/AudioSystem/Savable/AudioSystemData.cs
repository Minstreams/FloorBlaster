using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameSystem
{
    namespace Savable
    {
        [CreateAssetMenu(fileName = "AudioSystemData", menuName = "Savable/AudioSystemData")]
        public class AudioSystemData : SavableObject
        {
            [MinsHeader("音频系统可控配置", SummaryType.Title)]
            [LabelRange("Music Volume", 0, 1)]
            public float musicVolume;
            [LabelRange("Sound Volume", 0, 1)]
            public float soundVolume;

            public override void ApplyData()
            {
                AudioSystem.SetMusicVolume(musicVolume);
                AudioSystem.SetSoundVolume(soundVolume);
            }

            public override void UpdateData()
            {
                AudioSystem.Setting.mainMixer.GetFloat("MusicVolume", out musicVolume);
                AudioSystem.Setting.mainMixer.GetFloat("SoundVolume", out soundVolume);
            }
        }
    }
}
