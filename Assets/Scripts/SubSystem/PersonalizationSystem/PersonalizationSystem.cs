using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Setting;

namespace GameSystem
{
    /// <summary>
    /// 个性化系统，用于定义玩家个性化信息
    /// </summary>
    public class PersonalizationSystem : SubSystem<PersonalizationSystemSetting>
    {
        //Your code here
        /// <summary>
        /// 玩家个性化数据
        /// </summary>
        [System.Serializable]
        public class PlayerInfo
        {
            public string name;
            public Color skin;

            public bool Equals(PlayerInfo rhs)
            {
                return name.Equals(rhs.name) && skin.Equals(rhs.skin);
            }
        }

        /// <summary>
        /// 房间个性化数据
        /// </summary>
        [System.Serializable]
        public class RoomInfo
        {
            public string name;
        }
        public static PlayerInfo LocalPlayerInfo => Setting.localPlayerInfo;
        public static RoomInfo LocalRoomInfo => Setting.localRoomInfo;


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
