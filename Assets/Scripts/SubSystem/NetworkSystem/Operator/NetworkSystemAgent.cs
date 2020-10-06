using UnityEngine;

namespace GameSystem.Operator
{
    /// <summary>
    /// 代理网络系统的各种功能
    /// </summary>
    [AddComponentMenu("[NetworkSystem]/Operator/NetworkSystemAgent")]
    public class NetworkSystemAgent : MonoBehaviour
    {
#if UNITY_EDITOR
        [MinsHeader("Operator of NetworkSystem", SummaryType.PreTitleOperator, -1)]
        [MinsHeader("网络系统代理", SummaryType.TitleOrange, 0)]
        [MinsHeader("代理网络系统的各种功能", SummaryType.CommentCenter, 1)]
        [ConditionalShow, SerializeField] private bool useless; //在没有数据的时候让标题正常显示
#endif

        private void Awake()
        {
            NetworkSystem.OnUDPReceive += val => OnUDPReceive?.Invoke(val.message);
            NetworkSystem.OnReceive += val => OnReceive?.Invoke(val);
            NetworkSystem.OnConnected += () => OnConnected?.Invoke();
            NetworkSystem.OnDisconnected += () => OnDisconnected?.Invoke();
        }

        //Input
        [ContextMenu("LaunchServer")]
        public void LaunchServer() { NetworkSystem.LaunchServer(); }
        [ContextMenu("LaunchClient")]
        public void LaunchClient() { NetworkSystem.LaunchClient(); }
        [ContextMenu("ShutdownServer")]
        public void ShutdownServer() { NetworkSystem.ShutdownServer(); }
        [ContextMenu("ShutdownClient")]
        public void ShutdownClient() { NetworkSystem.ShutdownClient(); }

        //Output
        [MinsHeader("Events", SummaryType.Header, 2)]
        public StringEvent OnUDPReceive;
        public StringEvent OnReceive;
        public SimpleEvent OnConnected;
        public SimpleEvent OnDisconnected;
    }
}