using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Networking;

namespace GameSystem
{
    namespace Operator
    {
        /// <summary>
        /// 用于服务器向特定客户端发送和接收tcp消息
        /// </summary>
        [AddComponentMenu("[NetworkSystem]/Operator/ServerConnectionAgent")]
        public class ServerConnectionAgent : MonoBehaviour
        {
#if UNITY_EDITOR
            [MinsHeader("Operator of NetworkSystem", SummaryType.PreTitleOperator, -1)]
            [MinsHeader("服务器连接代理", SummaryType.TitleOrange, 0)]
            [MinsHeader("用于服务器向特定客户端发送和接收tcp消息", SummaryType.CommentCenter, 1)]
            [ConditionalShow, SerializeField] private bool useless; //在没有数据的时候让标题正常显示
#endif
            //Data
            [MinsHeader("Data", SummaryType.Header, 2)]
            [Label]
            public string message;

            //Output
            [MinsHeader("Output", SummaryType.Header, 2)]
            public StringEvent output;


            private Server.Connection connection;
            public void Init(Server.Connection connection)
            {
                this.connection = connection;
                connection.onReceive += msg => output?.Invoke(msg);
                NetworkSystem.server.onUDPReceive += msg => output?.Invoke(msg.message);
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
            [ContextMenu("UDPSend")]
            public void UDPSend()
            {
                NetworkSystem.server.UDPSend(message, connection.UDPEndPoint);
            }
        }
    }
}