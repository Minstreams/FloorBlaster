using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Operator
    {
        /// <summary>
        /// 用于客户端向服务器发送和接收tcp消息
        /// </summary>
        [AddComponentMenu("[NetworkSystem]/Operator/ClientConnectionAgent")]
        public class ClientConnectionAgent : MonoBehaviour
        {
#if UNITY_EDITOR
            [MinsHeader("Operator of NetworkSystem", SummaryType.PreTitleOperator, -1)]
            [MinsHeader("客户端连接代理", SummaryType.TitleOrange, 0)]
            [MinsHeader("用于客户端向服务器发送和接收tcp消息", SummaryType.CommentCenter, 1)]
            [ConditionalShow, SerializeField] private bool useless; //在没有数据的时候让标题正常显示
#endif

            //Data
            [MinsHeader("Data", SummaryType.Header, 2)]
            [Label]
            public string message;

            //Output
            [MinsHeader("Output", SummaryType.Header, 2)]
            public StringEvent output;


            private Networking.Client connection;
            [ContextMenu("Init")]
            public void Init()
            {
                connection = NetworkSystem.client;
                NetworkSystem.OnReceive += msg => output?.Invoke(msg);
                NetworkSystem.OnUDPReceive += packet => output?.Invoke(packet.message);
            }

            //Input
            [ContextMenu("Send")]
            public void Send()
            {
                Send(message);
            }
            public void Send(string message)
            {
                if (connection == null)
                {
                    Debug.LogWarning("No connection assigned");
                    return;
                }
                connection.Send(message);
            }
        }
    }
}