namespace GameSystem.Networking.Packet
{
    public class PacketToId<T> : PacketToId
    {
        public PacketToId(string id) : base(id)
        {
            this.pktTypeStr = typeof(T).FullName;
        }
    }
    public class PacketToId : PacketBase
    {
        public string id;
        public PacketToId(string id)
        {
            this.id = id;
        }
    }
}