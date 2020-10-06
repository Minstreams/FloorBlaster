using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Setting;

namespace GameSystem
{
    /// <summary>
    /// 管理音效的系统
    /// </summary>
    public class AudioSystem : SubSystem<AudioSystemSetting>
    {
        /// <summary>
        /// 背景音乐音源
        /// </summary>
        static AudioSource musicSource;
        static GameObject audioObject { get { return TheMatrix.Instance.gameObject; } }

        /// <summary>
        /// 配置音源效果的结构体
        /// </summary>
        [System.Serializable]
        public class AudioConfig
        {
            [Label]
            public bool loop;
            [LabelRange(-3, 3)]
            public float pitch;
            [LabelRange(0, 1)]
            public float volume;
            public AudioConfig()
            {
                this.loop = false;
                this.pitch = 1;
                this.volume = 1;
            }
            public AudioConfig(bool loop, float pitch, float volume)
            {
                this.loop = loop;
                this.pitch = pitch;
                this.volume = volume;
            }
            public void ApplyTo(AudioSource source)
            {
                source.loop = loop;
                source.pitch = pitch;
                source.volume = volume;
            }
        }

        /// <summary>
        /// 游戏中所有出现的音效
        /// </summary>
        public enum Sound
        {
            soundA,
            SoundB,
            s,
            c
        }

        /// <summary>
        /// 延迟执行
        /// </summary>
        static void DelayAction(float delay, System.Action action) { StartCoroutine(_DelayAction(delay, action)); }
        static IEnumerator _DelayAction(float delay, System.Action action)
        {
            yield return new WaitForSeconds(delay);
            action();
        }

        static List<AudioSource> soundsPlaying = new List<AudioSource>();
        static List<AudioSource> soundsLooping = new List<AudioSource>();


        //API---------------------------------
        public static void ChangeMusic(AudioClip clip, AudioConfig config)
        {
            musicSource.Stop();
            musicSource.clip = clip;
            config.ApplyTo(musicSource);
            musicSource.Play();
        }
        public static void PlaySound(AudioClip clip, AudioConfig config)
        {
            var source = audioObject.AddComponent<AudioSource>();
            source.clip = clip;
            config.ApplyTo(source);
            source.outputAudioMixerGroup = Setting.soundGroup;
            source.Play();
            if (config.loop)
            {
                soundsLooping.Add(source);
            }
            else
            {
                soundsPlaying.Add(source);
                DelayAction(clip.length, () => { soundsPlaying.Remove(source); MonoBehaviour.Destroy(source); });
            }
        }
        /// <summary>
        /// 停止所有在循环的音效
        /// </summary>
        public static void StopLooping()
        {
            for (int i = soundsLooping.Count - 1; i >= 0; --i)
            {
                var src = soundsLooping[i];
                soundsLooping.Remove(src);
                src.Stop();
                MonoBehaviour.Destroy(src);
            }
        }
        /// <summary>
        /// 停止所有音效
        /// </summary>
        public static void StopAllSounds()
        {
            for (int i = soundsPlaying.Count - 1; i >= 0; --i)
            {
                var src = soundsPlaying[i];
                soundsPlaying.Remove(src);
                src.Stop();
                MonoBehaviour.Destroy(src);
            }
            StopLooping();
        }
        public static void SetMusicVolume(float volume)
        {
            Setting.mainMixer.SetFloat("MusicVolume", volume);
        }
        public static void SetSoundVolume(float volume)
        {
            Setting.mainMixer.SetFloat("SoundVolume", volume);
        }


        [RuntimeInitializeOnLoadMethod]
        static void RuntimeInit()
        {
            //用于控制Action初始化
            TheMatrix.onGameAwake += OnGameAwake;
            TheMatrix.onGameStart += OnGameStart;
        }
        static void OnGameAwake()
        {
            //在进入游戏第一个场景时调用
            musicSource = audioObject.AddComponent<AudioSource>();
            musicSource.outputAudioMixerGroup = Setting.musicGroup;
        }
        static void OnGameStart()
        {
            //在主场景游戏开始时和游戏重新开始时调用
        }
    }
}
