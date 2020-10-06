using UnityEngine;

namespace GameSystem.Operator
{
    [AddComponentMenu("[AudioSystem]/Operator/AudioSystemOperator")]
    public class AudioSystemOperator : MonoBehaviour
    {
#if UNITY_EDITOR
        [MinsHeader("Operator of AudioSystem", SummaryType.PreTitleOperator, -1)]
        [MinsHeader("Audio System Operator", SummaryType.TitleOrange, 0)]
        [MinsHeader("声音系统的操作节点", SummaryType.CommentCenter, 1)]
        [ConditionalShow, SerializeField] bool useless;
#endif

        //Input
        [ContextMenu("StopLooping")]
        public void StopLooping()
        {
            AudioSystem.StopLooping();
        }
        [ContextMenu("StopAllSounds")]
        public void StopAllSounds()
        {
            AudioSystem.StopAllSounds();
        }
    }
}