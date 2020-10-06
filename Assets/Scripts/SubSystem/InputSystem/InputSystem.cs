using System.Collections;
using UnityEngine;
using GameSystem.Setting;


namespace GameSystem
{
    /// <summary>
    /// 封装输入的系统
    /// </summary>
    public class InputSystem : SubSystem<InputSystemSetting>
    {
        // Actions----------------------------
        // 在此添加新的Action
        /// <summary>
        /// 所有输入行为的枚举
        /// </summary>
        public enum InputAction
        {
            /// <summary>
            /// 任意输入，持续响应
            /// </summary>
            Any,
            /// <summary>
            /// 任意输入
            /// </summary>
            AnyDown,
            /// <summary>
            /// 移动行为
            /// </summary>
            Move,
        }
        /// <summary>
        /// 任意输入，持续响应
        /// </summary>
        public static event System.Action _Any;
        /// <summary>
        /// 任意输入
        /// </summary>
        public static event System.Action _AnyDown;
        /// <summary>
        /// 移动行为
        /// </summary>
        public static event System.Action<Vector2> _Move;
        public static void Move(Vector2 val) { _Move?.Invoke(val); }

        // States-----------------------------
        /// <summary>
        /// 通用输入逻辑
        /// </summary>
        static void GeneralLogic()
        {
            if (Input.anyKey) _Any?.Invoke();
            if (Input.anyKeyDown) _AnyDown?.Invoke();
        }

        // 在此定义新的状态（处理跨平台输入）
        public class MoveState : InputState
        {
            public override void Execute()
            {
                Vector2 output = Vector2.zero;

                if (GetKey(InputKey.Left)) output.x -= 1;
                if (GetKey(InputKey.Right)) output.x += 1;
                if (GetKey(InputKey.Up)) output.y += 1;
                if (GetKey(InputKey.Down)) output.y -= 1;
                if (output.sqrMagnitude > 1) output.Normalize();

                _Move?.Invoke(output);
            }
        }


        #region Action状态机 ====================================
        // 状态机-----------------------------
        /// <summary>
        /// 继承此类实现特定输入状态机
        /// </summary>
        public abstract class InputState
        {
            /// <summary>
            /// 每帧调用
            /// </summary>
            public abstract void Execute();
            /// <summary>
            /// 进入状态时调用
            /// </summary>
            public virtual void OnEnter() { }
            /// <summary>
            /// 退出状态时调用
            /// </summary>
            public virtual void OnExit() { }
        }

        static InputState currentState;
        class DefaultInputState : InputState
        {
            public override void Execute() { }
        }
        static IEnumerator StateMachineProcess()
        {
            Debug.Log("Input State Machine Launched!");
            currentState = new DefaultInputState();
            while (true)
            {
                yield return 0;
                GeneralLogic();
                currentState.Execute();
            }
        }


        // API--------------------------------
        /// <summary>
        /// 给TheMatrix调用，改变输入系统状态
        /// </summary>
        public static void ChangeState(InputState state)
        {
            currentState.OnExit();
            currentState = state;
            currentState.OnEnter();
        }
        #endregion

        #region 输入层 - Keyboard ===============================
        //所有输入按键种类
        public enum InputKey
        {
            Up,
            Down,
            Left,
            Right,
        }

        //API---------------------------------
        public static bool GetKey(InputKey input)
        {
            return Input.GetKey(Setting.MainKeys[input]) || Input.GetKey(Setting.SideKeys[input]);
        }
        public static bool GetKeyDown(InputKey input)
        {
            return Input.GetKeyDown(Setting.MainKeys[input]) || Input.GetKeyDown(Setting.SideKeys[input]);
        }
        public static bool GetKeyUp(InputKey input)
        {
            return Input.GetKeyUp(Setting.MainKeys[input]) || Input.GetKeyUp(Setting.SideKeys[input]);
        }
        #endregion

        #region 流程 ============================================
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        static void RuntimeInit()
        {
            //用于控制Action初始化
            TheMatrix.onGameAwake += OnGameAwake;
        }
        static void OnGameAwake()
        {
            //在进入游戏第一个场景时调用
            StartCoroutine(StateMachineProcess());
        }
        #endregion
    }
}