using System;

namespace GameSystem.Networking
{
    /// <summary>
    /// 服务器端有Connection连接上时
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    class TCPConnectionProcessAttribute : Attribute { }
}