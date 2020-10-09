using System.Collections.Generic;
using UnityEngine;
using GameSystem.Setting;

namespace GameSystem
{
    /// <summary>
    /// 控制游戏对战流程和数据结构
    /// </summary>
    public class GameplaySystem : SubSystem<GameplaySystemSetting>
    {
        public static Dictionary<string, PlayerAvater> playersDatabase = new Dictionary<string, PlayerAvater>();

        public static Transform playerRoot;

        static Vector3 NewPlayerPos()
        {
            return new Vector3(4, 0.5f, 4);
        }
        public static void AddPlayer(string id, PersonalizationSystem.PlayerInfo info) => AddPlayer(id, info, NewPlayerPos());

        public static void AddPlayer(string id, PersonalizationSystem.PlayerInfo info, Vector3 pos)
        {
            var g = GameObject.Instantiate(Setting.playerPrefab, pos, Quaternion.identity, playerRoot);
            var avater = g.GetComponent<PlayerAvater>();
            avater.info = info;
            avater.targetPosition = pos;
            avater.ActivateId(id);
            playersDatabase.Add(avater.netId, avater);
        }
        public static void DeletePlayer(string id)
        {
            if (!playersDatabase.ContainsKey(id)) throw new System.Exception("正在试图删除一个不存在的玩家。ID：" + id);
            GameObject.Destroy(playersDatabase[id].gameObject);
            playersDatabase.Remove(id);
        }

        [RuntimeInitializeOnLoadMethod]
        static void RuntimeInit()
        {
            //用于控制Action初始化
            TheMatrix.onGameAwake += OnGameAwake;
            TheMatrix.onGameReady += OnGameReady;
            TheMatrix.onGameStart += OnGameStart;
        }
        static void OnGameAwake()
        {
            //在进入游戏第一个场景时调用
        }
        static void OnGameReady()
        {
            //进入Room时调用
            playersDatabase.Clear();
            playersDatabase.Add(NetworkSystem.netId, PlayerAvater.local);
        }
        static void OnGameStart()
        {
            //在主场景游戏开始时和游戏重新开始时调用
        }


        //API---------------------------------
        //public static void SomeFunction(){}
    }
}
