using System;

namespace GameSystem.Networking
{
    /// <summary>
    /// TCP断连时
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    class TCPDisconnectionAttribute : Attribute { }
}