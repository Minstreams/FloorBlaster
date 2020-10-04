namespace GameSystem
{
    namespace Networking
    {
        public struct TCPPacket
        {
            public string message;
            public Server.Connection connection;
            public TCPPacket(string message, Server.Connection connection)
            {
                this.message = message;
                this.connection = connection;
            }
        }
    }
}
