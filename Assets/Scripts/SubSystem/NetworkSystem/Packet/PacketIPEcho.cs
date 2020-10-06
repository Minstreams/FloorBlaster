using System.Net;
namespace GameSystem.Networking.Packet
{
    /// <summary>
    /// IP 回声定位器
    /// </summary>
    public class PacketIPEcho : Packet<PacketIPEcho>
    {
        public string addressStr;
        public IPAddress address
        {
            get => IPAddress.Parse(addressStr);
            set => addressStr = value.ToString();
        }
        public PacketIPEcho(IPAddress address) : base()
        {
            this.address = address;
        }
    }
}