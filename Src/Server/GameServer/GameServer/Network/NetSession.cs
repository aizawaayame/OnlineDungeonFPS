using GameServer.Entities;
using Protocol;

namespace GameServer.Network
{
    class NetSession
    {
        public TUser User { get; set; }
        public Character Character { get; set; }
        public NEntity Entity { get; set; }
    }
}
