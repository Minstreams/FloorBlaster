namespace GameSystem
{
    namespace Networking
    {
        namespace Packet
        {
            public class Packet<T> : PacketBase
            {

                public Packet()
                {
                    this.pktTypeStr = typeof(T).FullName;
                }
            }
        }
    }
}
