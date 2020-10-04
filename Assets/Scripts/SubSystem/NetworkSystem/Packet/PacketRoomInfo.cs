namespace GameSystem
{
    namespace Networking
    {
        namespace Packet
        {
            public class PacketRoomInfo : Packet<PacketRoomInfo>
            {
                public PacketRoomInfo(string title) : base()
                {
                    roomTitle = title;
                }
                public string roomTitle;
            }
        }
    }
}
