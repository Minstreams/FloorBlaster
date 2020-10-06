namespace GameSystem.Networking.Packet
{
    public class Packet<T> : PacketBase
    {

        public Packet()
        {
            this.pktTypeStr = typeof(T).FullName;
        }
    }
}