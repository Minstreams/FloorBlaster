using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Setting;
using UnityEngine.SceneManagement;

namespace GameSystem
{
    /// <summary>
    /// 场景系统，用于加载卸载场景
    /// </summary>
    public class SceneSystem : SubSystem<SceneSystemSetting>
    {
        public static event System.Action OnPendingLoadScene;
        public static TheMatrix.GameScene sceneToLoad { private set; get; }

        static bool loadConfirmed;
        public static void ConfirmLoadScene()
        {
            loadConfirmed = true;
        }
        public static IEnumerator LoadScene(TheMatrix.GameScene gameScene)
        {
            loadConfirmed = false;
            sceneToLoad = gameScene;
            OnPendingLoadScene?.Invoke();
            Log("Pending Load Scene: " + gameScene);
            while (!loadConfirmed)
            {
                yield return 0;
            }
            Log("Load Confirmed!");
            SceneManager.LoadScene(TheMatrix.GetScene(gameScene));
        }
    }
}