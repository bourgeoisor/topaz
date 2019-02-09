using Lidgren.Network;
using Topaz.Entity;

namespace Topaz.Networking
{
    class ServerConnection
    {
        public NetConnection NetConnection { get; set; }
        internal Player Player { get; set; }

        public ServerConnection(NetConnection connection)
        {
            NetConnection = connection;
            Player = new Player();
        }
    }
}
