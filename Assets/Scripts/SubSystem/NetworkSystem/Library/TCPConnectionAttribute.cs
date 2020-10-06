using System;

namespace GameSystem.Networking
{
    /// <summary>
    /// TCP连接上时
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    class TCPConnectionAttribute : Attribute { }
}