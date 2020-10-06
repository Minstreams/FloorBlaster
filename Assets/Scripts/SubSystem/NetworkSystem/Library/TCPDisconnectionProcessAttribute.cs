using System;

namespace GameSystem.Networking
{
    /// <summary>
    /// 服务器端有Connection断连时
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    class TCPDisconnectionProcessAttribute : Attribute { }
}