using UnityEngine;

namespace GameSystem.Operator
{
    [AddComponentMenu("[AudioSystem]/Operator/SoundPlayer")]
    public class SoundPlayer : Linker.AudioConfigLinker
    {
#if UNITY_EDITOR
        [MinsHeader("Operator of AudioSystem", SummaryType.PreTitleOperator, -1)]
        [MinsHeader("Sound Player", SummaryType.TitleOrange, 0)]
        [MinsHeader("此操作节点用来播放音效", SummaryType.CommentCenter, 1)]
        [ConditionalShow, System.NonSerialized] bool useless;
#endif
        //Data
        [MinsHeader("Data", SummaryType.Header, 2)]
        [Label]
        public AudioSystem.Sound sound;

        //Input
        [ContextMenu("Play Sound")]
        public override void Invoke()
        {
            base.Invoke();
            AudioSystem.PlaySound(AudioSystem.Setting.soundClips[sound], data);
        }
        public void SetClipIndex(AudioSystem.Sound sound)
        {
            this.sound = sound;
        }
    }
}