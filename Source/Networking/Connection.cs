using Lidgren.Network;
using Topaz.Mob;

namespace Topaz.Networking
{
    class Connection
    {
        NetConnection _netConnection;
        Mob.Player _player;

        public Connection(NetConnection connection)
        {
            _netConnection = connection;
            _player = new Player();
        }

        public NetConnection NetConnection { get => _netConnection; set => _netConnection = value; }
        internal Player Player { get => _player; set => _player = value; }
    }
}
