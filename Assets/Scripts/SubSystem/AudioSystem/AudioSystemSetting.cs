using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GameSystem.Setting
{
    [CreateAssetMenu(fileName = "AudioSystemSetting", menuName = "系统配置文件/AudioSystemSetting")]
    public class AudioSystemSetting : ScriptableObject
    {
        [MinsHeader("AudioSystem Setting", SummaryType.Title, -2)]
        [MinsHeader("管理音效的系统", SummaryType.CommentCenter, -1)]

        [MinsHeader("声音通道", SummaryType.Header), Space(16)]
        [Label]
        public AudioMixer mainMixer;
        [Label]
        public AudioMixerGroup musicGroup;
        [Label]
        public AudioMixerGroup soundGroup;

        [MinsHeader("音乐文件", SummaryType.Header), Space(16)]
        [Label("Clip")]
        public List<AudioClip> musicClips;

        [MinsHeader("音效文件", SummaryType.Header), Space(16)]
        public SoundClipMap soundClips;
    }
}