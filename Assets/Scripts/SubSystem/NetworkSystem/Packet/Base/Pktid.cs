/// <summary>
/// 发送给指定id的包
/// 命名前缀：Si
/// </summary>
public class Pktid<T> : Pktid
{
    public Pktid(string id) : base(id)
    {
        this.pktTypeStr = typeof(T).FullName;
    }
}
/// <summary>
/// 发送给指定id的包
/// </summary>
public class Pktid : PacketBase
{
    public string id;
    public Pktid(string id)
    {
        this.id = id;
    }
}