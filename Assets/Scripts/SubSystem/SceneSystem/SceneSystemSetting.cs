using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSystem
{
    namespace Setting
    {
        [CreateAssetMenu(fileName = "SceneSystemSetting", menuName = "系统配置文件/SceneSystemSetting")]
        public class SceneSystemSetting : ScriptableObject
        {
            //[MinsHeader("SceneSystem Setting", SummaryType.Title, -2)]
            //[MinsHeader("场景系统，用于加载卸载场景", SummaryType.CommentCenter, -1)]

            //[MinsHeader("场景信息", SummaryType.Header), Space(16)]
            [Label("加载过程场景")]
            public string loadingScene;
        }
    }
}
