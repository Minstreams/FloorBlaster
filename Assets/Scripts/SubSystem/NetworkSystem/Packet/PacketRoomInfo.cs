namespace GameSystem
{
    namespace Networking
    {
        namespace Packet
        {
            public class PacketRoomInfo : Packet<PacketRoomInfo>
            {
                public string roomTitle;
                public PacketRoomInfo(string title) : base()
                {
                    roomTitle = title;
                }
            }
        }
    }
}
