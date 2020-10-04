using System;

namespace GameSystem
{
    namespace Networking
    {
        public class PacketBase
        {
            public string pktTypeStr;
            public Type pktType { get { return Type.GetType(pktTypeStr); } }
            public bool MatchType(Type type)
            {
                return type.FullName == pktTypeStr;
            }
        }
    }
}
