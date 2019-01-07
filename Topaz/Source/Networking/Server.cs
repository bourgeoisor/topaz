using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Topaz.Networking
{
    public sealed class Server
    {
        Engine.Logger _logger = new Engine.Logger("Server");

        Thread _thread;
        NetServer _server;

        Dictionary<long, Networking.Connection> _connections;
        World.Chunk _map;

        private static readonly Lazy<Server> lazy =
            new Lazy<Server>(() => new Server());

        public static Server Instance { get { return lazy.Value; } }

        private Server()
        {
        }

        public void Initialize()
        {
            _logger.Info("Starting...");

            _map = new World.Chunk();
            _map.GenerateRandom();
            _connections = new Dictionary<long, Networking.Connection>();

            NetPeerConfiguration config = new NetPeerConfiguration(Properties.Resources.Title);
            config.Port = 12345;
            config.EnableUPnP = true;
            config.ConnectionTimeout = 10;

            _server = new NetServer(config);
            _server.Start();

            _thread = new Thread(ServerThread);
            _thread.Start();

            _logger.Info("Started.");
        }

        public void Terminate()
        {
            if (_thread != null)
            {
                _server.Shutdown("terminating");
                _thread.Abort();
            }
        }

        public void ForwardPort()
        {
            _server.UPnP.ForwardPort(12345, Properties.Resources.Title);
        }

        public void ServerThread()
        {   
            NetIncomingMessage msg;
            while (Engine.Window.Instance.State == Engine.Window.WindowState.Running)
            {
                Thread.Sleep(5);
                while ((msg = _server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.StatusChanged:
                            _logger.Info("StatusChanged received: " + msg.SenderConnection.Status);
                            HandleStatusChanged(msg);
                            break;
                        case NetIncomingMessageType.Data:
                            HandleIncomingData(msg);
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                            _logger.Debug(msg.ReadString());
                            break;
                        case NetIncomingMessageType.WarningMessage:
                            _logger.Warn(msg.ReadString());
                            break;
                        case NetIncomingMessageType.ErrorMessage:
                            _logger.Error(msg.ReadString());
                            break;
                        default:
                            _logger.Warn("Unhandled MessageType: " + msg.MessageType);
                            break;
                    }
                    _server.Recycle(msg);
                }
            }

            _logger.Info("Terminated.");
        }

        public void HandleStatusChanged(NetIncomingMessage msg)
        {
            if (msg.SenderConnection.Status == NetConnectionStatus.Connected)
            {
                _connections.Add(msg.SenderConnection.RemoteUniqueIdentifier, new Networking.Connection(msg.SenderConnection));
                SendConnectionInfo(msg.SenderConnection);
                SendMap(msg.SenderConnection);
                SendPlayerInfo(_connections[msg.SenderConnection.RemoteUniqueIdentifier]);
            }

            if (msg.SenderConnection.Status == NetConnectionStatus.Disconnected)
            {
                _connections.Remove(msg.SenderConnection.RemoteUniqueIdentifier);
                SendPlayerDisconnected(msg.SenderConnection);
            }
        }

        public void HandleIncomingData(NetIncomingMessage msg)
        {
            MessageType type = (MessageType)msg.ReadInt32();

            if (type == MessageType.PlayerMoved)
            {
                float x = msg.ReadFloat();
                float y = msg.ReadFloat();
                Mob.Mob.Direction direction = (Mob.Mob.Direction)msg.ReadInt32();

                _connections[msg.SenderConnection.RemoteUniqueIdentifier].Player.SetCoordinates(new Vector2(x, y));
                _connections[msg.SenderConnection.RemoteUniqueIdentifier].Player.SetDirection(direction);

                SendPlayerMove(msg.SenderConnection.RemoteUniqueIdentifier);
            }

            if (type == MessageType.MapChanged)
            {
                int j = msg.ReadInt32();
                int i = msg.ReadInt32();
                int tile = msg.ReadInt32();

                _map.Layer2[j, i] = tile;

                SendMapChange(j, i, tile);
            }
        }

        public NetOutgoingMessage CreateMessage(MessageType type)
        {
            NetOutgoingMessage msg = _server.CreateMessage();
            msg.Write((int)type);
            return msg;
        }

        public void SendMessage(NetOutgoingMessage msg, NetConnection recipient, NetDeliveryMethod method, int sequenceChannel)
        {
            _server.SendMessage(msg, recipient, method, sequenceChannel);
        }

        public void SendMessage(NetOutgoingMessage msg, List<NetConnection> recipients, NetDeliveryMethod method, int sequenceChannel)
        {
            if (recipients.Count == 0)
                return;

            _server.SendMessage(msg, recipients, method, sequenceChannel);
        }

        public void SendConnectionInfo(NetConnection connection)
        {
            NetOutgoingMessage msg = CreateMessage(MessageType.ConnectionInfo);

            msg.Write(connection.RemoteUniqueIdentifier);

            _server.SendMessage(msg, connection, NetDeliveryMethod.ReliableOrdered, 1);
        }

        public void SendMap(NetConnection connection)
        {
            NetOutgoingMessage msg = CreateMessage(MessageType.MapInfo);

            msg.Write(_map.Layer1.GetLength(0));
            msg.Write(_map.Layer1.GetLength(1));
            for (int j = 0; j < _map.Layer1.GetLength(0); j++)
            {
                for (int i = 0; i < _map.Layer1.GetLength(1); i++)
                {
                    msg.Write(_map.Layer1[j, i]);
                }
            }
            for (int j = 0; j < _map.Layer2.GetLength(0); j++)
            {
                for (int i = 0; i < _map.Layer2.GetLength(1); i++)
                {
                    msg.Write(_map.Layer2[j, i]);
                }
            }

            _server.SendMessage(msg, connection, NetDeliveryMethod.ReliableOrdered, 1);
        }

        private void SendPlayerInfo(Networking.Connection connection)
        {
            List<NetConnection> recipients = new List<NetConnection>();

            foreach (long id in _connections.Keys)
            {
                if (id != connection.NetConnection.RemoteUniqueIdentifier)
                {
                    recipients.Add(_connections[id].NetConnection);

                    // Send existing players to newly connected player
                    NetOutgoingMessage msg = CreateMessage(MessageType.PlayerConnected);

                    msg.Write(id);
                    msg.Write(_connections[id].Player.GetCoordinates().X);
                    msg.Write(_connections[id].Player.GetCoordinates().Y);
                    msg.Write((int)_connections[id].Player.GetDirection());

                    SendMessage(msg, connection.NetConnection, NetDeliveryMethod.ReliableOrdered, 1);
                }
            }

            // Send newly connected player to other players.
            {
                NetOutgoingMessage msg = CreateMessage(MessageType.PlayerConnected);

                msg.Write(connection.NetConnection.RemoteUniqueIdentifier);
                msg.Write(connection.Player.GetCoordinates().X);
                msg.Write(connection.Player.GetCoordinates().Y);
                msg.Write((int)connection.Player.GetDirection());

                SendMessage(msg, recipients, NetDeliveryMethod.ReliableOrdered, 1);
            }
        }

        public void SendPlayerDisconnected(NetConnection connection)
        {
            List<NetConnection> recipients = new List<NetConnection>();
            foreach (long id in _connections.Keys)
                recipients.Add(_connections[id].NetConnection);

            foreach (long id in _connections.Keys)
            {
                NetOutgoingMessage msg = CreateMessage(MessageType.PlayerDisconnected);

                msg.Write(connection.RemoteUniqueIdentifier);

                SendMessage(msg, recipients, NetDeliveryMethod.ReliableOrdered, 2);
            }
        }

        public void SendPlayerMove(long srcId)
        {
            List<NetConnection> recipients = new List<NetConnection>();
            foreach (long id in _connections.Keys)
                recipients.Add(_connections[id].NetConnection);

            NetOutgoingMessage nmsg = CreateMessage(MessageType.PlayerMoved);
            nmsg.Write(srcId);
            nmsg.Write(_connections[srcId].Player.GetCoordinates().X);
            nmsg.Write(_connections[srcId].Player.GetCoordinates().Y);
            nmsg.Write((int)_connections[srcId].Player.GetDirection());

            _server.SendMessage(nmsg, recipients, NetDeliveryMethod.ReliableOrdered, 2);
        }

        public void SendMapChange(int j, int i, int tile)
        {
            List<NetConnection> recipients = new List<NetConnection>();
            foreach (long id in _connections.Keys)
                recipients.Add(_connections[id].NetConnection);
            
            NetOutgoingMessage nmsg = CreateMessage(MessageType.MapChanged);

            nmsg.Write(j);
            nmsg.Write(i);
            nmsg.Write(_map.Layer2[j, i]);

            _server.SendMessage(nmsg, recipients, NetDeliveryMethod.ReliableOrdered, 2);
        }
    }
}
