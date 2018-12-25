using Lidgren.Network;
using Topaz.Mob;

namespace Topaz.Networking
{
    class Connection
    {
        NetConnection netConnection;
        Mob.Player player;

        public Connection(NetConnection connection)
        {
            this.NetConnection = connection;
            this.player = new Player();
        }

        public NetConnection NetConnection { get => netConnection; set => netConnection = value; }
        internal Player Player { get => player; set => player = value; }
    }
}
