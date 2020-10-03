using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameSystem
{
    namespace Operator
    {
        [AddComponentMenu("[TheMatrix]/Operator/GameMessageSender")]
        public class GameMessageSender : MonoBehaviour
        {
#if UNITY_EDITOR
            [MinsHeader("Operator of TheMatrix", SummaryType.PreTitleOperator, -1)]
            [MinsHeader("游戏消息发送器", SummaryType.TitleOrange, 0)]
            [MinsHeader("此操作节点用来发送游戏消息", SummaryType.CommentCenter, 1)]
            [ConditionalShow, SerializeField] private bool useless;
#endif

            //Data
            [MinsHeader("Data", SummaryType.Header, 2)]
            [Label]
            public GameMessage message;
            [Label(true)]
            public bool sendOnStart;

            private void Start()
            {
                if (sendOnStart) SendGameMessage();
            }

            //Input
            [ContextMenu("Send")]
            public void SendGameMessage()
            {
                TheMatrix.SendGameMessage(message);
            }
        }
    }
}
