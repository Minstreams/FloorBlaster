using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Operator
{
    /// <summary>
    /// 用来播放加载场景的动画
    /// </summary>
    [AddComponentMenu("[SceneSystem]/Operator/SceneLoadingConfirmer")]
    public class SceneLoadingConfirmer : MonoBehaviour
    {
#if UNITY_EDITOR
        [MinsHeader("Operator of SceneSystem", SummaryType.PreTitleOperator, -1)]
        [MinsHeader("场景加载确认器", SummaryType.TitleOrange, 0)]
        [MinsHeader("用来播放加载场景的动画", SummaryType.CommentCenter, 1)]
        [ConditionalShow, SerializeField] bool useless; //在没有数据的时候让标题正常显示
#endif

        //Data
        [MinsHeader("配置", SummaryType.Header, 2)]
        public TheMatrix.GameScene sceneToLoad;

        //Ouput
        public SimpleEvent onPendingLoadScece;

        //Input
        public void Confirm() => SceneSystem.ConfirmLoadScene();

        void OnPendingLoadScene()
        {
            if (SceneSystem.sceneToLoad == sceneToLoad) onPendingLoadScece?.Invoke();
        }

        private void Start()
        {
            SceneSystem.OnPendingLoadScene += OnPendingLoadScene;
        }
        private void OnDestroy()
        {
            SceneSystem.OnPendingLoadScene -= OnPendingLoadScene;
        }
    }
}