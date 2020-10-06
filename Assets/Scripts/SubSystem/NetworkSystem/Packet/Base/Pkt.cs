public class Pkt<T> : PacketBase
{

    public Pkt()
    {
        this.pktTypeStr = typeof(T).FullName;
    }
}