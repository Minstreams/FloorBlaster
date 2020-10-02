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
            [MinsHeader("SavableObject of AudioSystem", SummaryType.PreTitleSavable, -1)]
            [MinsHeader("音频系统可控配置", SummaryType.TitleGreen, 0)]
            [MinsHeader("用来存储用户的音频设置", SummaryType.CommentCenter, 1)]

            [LabelRange(0, 1)]
            public float musicVolume;
            [LabelRange(0, 1)]
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
