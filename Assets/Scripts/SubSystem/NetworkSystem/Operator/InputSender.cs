using GameSystem.Networking;
using System.Collections;
using UnityEngine;

namespace GameSystem.Operator
{
    /// <summary>
    /// 用来向服务器发送输入信息包
    /// </summary>
    [AddComponentMenu("[NetworkSystem]/Operator/InputSender")]
    public class InputSender : NetworkObject
    {
#if UNITY_EDITOR
        [MinsHeader("Operator of NetworkSystem", SummaryType.PreTitleOperator, -1)]
        [MinsHeader("输入发送器", SummaryType.TitleOrange, 0)]
        [MinsHeader("用来向服务器发送输入信息包", SummaryType.CommentCenter, 1)]
        [ConditionalShow, SerializeField] bool useless; //在没有数据的时候让标题正常显示
#endif

        //Data
        //[MinsHeader("Data", SummaryType.Header, 2)]

        private void Start()
        {
            InputSystem._Move += InputMove;
        }
        private protected override void OnDestroy()
        {
            InputSystem._Move -= InputMove;
            base.OnDestroy();
        }

        Vector2 lastMovement;
        Vector2 movement;
        float timer;
        private void Update()
        {
            timer += Time.deltaTime;

            // 连续移动时按照设定的间隔发送，输入变化超过阈值马上发送
            if (movement != lastMovement && (timer > Setting.inputSendInterval || Vector2.Distance(movement, lastMovement) > Setting.inputSendMoveThreadhold))
            {
                timer = 0;
                ClientSendPacket(new IMove(movement));
                lastMovement = movement;
            }
        }
        void InputMove(Vector2 input)
        {
            movement = input;
        }
    }
}