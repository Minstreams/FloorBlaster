using GameSystem.Networking;
using GameSystem.Networking.Packet;
using System.Collections;
using System.Collections.Generic;
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
            StartCoroutine(MoveSender());
        }
        private protected override void OnDestroy()
        {
            InputSystem._Move -= InputMove;
            base.OnDestroy();
        }

        Vector2 movement;
        void InputMove(Vector2 input)
        {
            movement = input;
        }
        IEnumerator MoveSender()
        {
            //todo 做如下优化
            //var interval = new WaitForSeconds(Setting.inputSendInterval);
            while (true)
            {
                yield return new WaitForSeconds(Setting.inputSendInterval);
                ClientSendPacket(new PacketInputMove(movement));
            }
        }
    }
}