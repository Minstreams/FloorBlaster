using UnityEngine;

namespace GameSystem.Operator
{
    [AddComponentMenu("[AudioSystem]/Operator/MusicChanger")]
    public class MusicChanger : Linker.AudioConfigLinker
    {
#if UNITY_EDITOR
        [MinsHeader("Operator of AudioSystem", SummaryType.PreTitleOperator, -1)]
        [MinsHeader("Music Changer", SummaryType.TitleOrange, 0)]
        [MinsHeader("此操作节点用来换音乐", SummaryType.CommentCenter, 1)]
        [ConditionalShow, System.NonSerialized] bool useless;
#endif

        //Data
        [MinsHeader("Data", SummaryType.Header, 2)]
        [Label]
        public int clipIndex;

        //Input
        [ContextMenu("Change Music")]
        public override void Invoke()
        {
            base.Invoke();
            AudioSystem.ChangeMusic(AudioSystem.Setting.musicClips[clipIndex], data);
        }
        public void SetClipIndex(int clipIndex)
        {
            this.clipIndex = clipIndex;
        }
    }
}