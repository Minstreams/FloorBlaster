using System;

namespace GameSystem.Networking
{
    /// <summary>
    /// 客户器端接受UDP消息
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    class UDPReceiveAttribute : Attribute { }
}