using System;

public class PacketBase
{
    /// <summary>
    /// typeString
    /// </summary>
    public string ts;
    public Type pktType { get { return Type.GetType(ts); } }
    public bool MatchType(Type type)
    {
        return type.FullName == ts;
    }
    public bool IsSubclassOf(Type type)
    {
        return pktType.IsSubclassOf(type);
    }
}