using System;

public class PacketBase
{
    public string pktTypeStr;
    public Type pktType { get { return Type.GetType(pktTypeStr); } }
    public bool MatchType(Type type)
    {
        return type.FullName == pktTypeStr;
    }
    public bool IsSubclassOf(Type type)
    {
        return type.IsSubclassOf(pktType);
    }
}