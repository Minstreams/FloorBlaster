using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Setting;

namespace GameSystem
{
    /// <summary>
    /// 摄像机系统
    /// </summary>
    public class CameraSystem : SubSystem<CameraSystemSetting>
    {
        //Your code here


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
        }
        static void OnGameStart()
        {
            //在主场景游戏开始时和游戏重新开始时调用
        }


        //API---------------------------------
        //public static void SomeFunction(){}
    }
}
