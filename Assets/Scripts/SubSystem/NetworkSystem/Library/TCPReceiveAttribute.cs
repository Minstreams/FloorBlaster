using System;

namespace GameSystem.Networking
{
    /// <summary>
    /// 客户端接受TCP消息
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    class TCPReceiveAttribute : Attribute { }
}