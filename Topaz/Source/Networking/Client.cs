using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Topaz.Mob;
using Topaz.World;

namespace Topaz.Networking
{
    public sealed class Client
    {
        Engine.Logger _logger = new Engine.Logger("Client");

        NetClient _client;
        World.Chunk _map;
        Mob.Player _player;
        Dictionary<long, Mob.Player> _players;
        string _lastNetMessage;
        int _lastLatency;
        long _localId;

        private static readonly Lazy<Client> lazy =
            new Lazy<Client>(() => new Client());

        public static Client Instance { get { return lazy.Value; } }

        internal Chunk Map { get => _map; set => _map = value; }
        internal Player Player { get => _player; set => _player = value; }
        internal Dictionary<long, Player> Players { get => _players; set => _players = value; }
        public int LastLatency { get => _lastLatency; set => _lastLatency = value; }
        public string LastNetMessage { get => _lastNetMessage; set => _lastNetMessage = value; }

        private Client()
        {
        }

        public void Initialize()
        {
            _logger.Info("Starting...");

            _map = new World.Chunk();
            _player = new Mob.Player();
            _players = new Dictionary<long, Mob.Player>();

            NetPeerConfiguration config = new NetPeerConfiguration(Properties.Resources.Title);
            config.ConnectionTimeout = 10;
            config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);

            _client = new NetClient(config);
            _client.Start();

            _logger.Info("Started.");
        }

        public void Connect(string host, int port)
        {
            _client.Connect(host: host, port: port);
        }

        public void Disconnect()
        {
            _client.Disconnect("terminating");
            _logger.Info("Disconnected.");
        }

        public NetConnectionStatus GetClientConnectionStatus()
        {
            return _client.ConnectionStatus;
        }

        public void HandleMessages()
        {
            NetIncomingMessage msg;
            while ((msg = _client.ReadMessage()) != null)
            {
                if (msg.PeekString() != "")
                    _lastNetMessage = msg.PeekString();

                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        _logger.Info("StatusChanged received: " + msg.SenderConnection.Status);
                        break;
                    case NetIncomingMessageType.Data:
                        Console.WriteLine(msg.PeekInt32());
                        _lastNetMessage = (MessageType)msg.PeekInt32() + "";
                        HandleIncomingData(msg);
                        break;
                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                        _lastLatency = (int)Math.Round(msg.ReadFloat()*1000);
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

                _client.Recycle(msg);
            }
        }

        void HandleIncomingData(NetIncomingMessage msg)
        {
            MessageType type = (MessageType)msg.ReadInt32();

            if (type == MessageType.ConnectionInfo)
            {
                _localId = msg.ReadInt64();
                _players.Add(_localId, new Player());
                _players[_localId].LoadContent();
                _players[_localId].DrawAtOrigin = true;
                _player = _players[_localId];
            }

            if (type == MessageType.PlayerDisconnected)
            {
                long id = msg.ReadInt64();
                _players.Remove(id);
            }

            if (type == MessageType.MapInfo)
            {
                int rows = msg.ReadInt32();
                int cols = msg.ReadInt32();

                _map.Layer1 = new int[rows, cols];
                _map.Layer2 = new int[rows, cols];

                for (int j = 0; j < rows; j++)
                    for (int i = 0; i < cols; i++)
                        _map.Layer1[j, i] = msg.ReadInt32();

                for (int j = 0; j < rows; j++)
                    for (int i = 0; i < cols; i++)
                        _map.Layer2[j, i] = msg.ReadInt32();
            }

            if (type == MessageType.PlayerConnected)
            {
                long id = msg.ReadInt64();
                float x = msg.ReadFloat();
                float y = msg.ReadFloat();
                Mob.Mob.Direction direction = (Mob.Mob.Direction)msg.ReadInt32();

                _players.Add(id, new Player());
                _players[id].LoadContent();
                _players[id].SetCoordinates(new Vector2(x, y));
                _players[id].SetDirection(direction);
            }

            if (type == MessageType.PlayerMoved)
            {
                long id = msg.ReadInt64();
                float x = msg.ReadFloat();
                float y = msg.ReadFloat();
                Mob.Mob.Direction direction = (Mob.Mob.Direction)msg.ReadInt32();

                if (_players.ContainsKey(id))
                {
                    if (id == _localId && direction == Mob.Mob.Direction.None)
                    {
                        _players[id].SetCoordinates(new Vector2(x, y));
                    }
                    else if (id != _localId)
                    {
                        _players[id].SetCoordinates(new Vector2(x, y));
                        _players[id].SetDirection(direction);
                    }
                    
                }
            }

            if (type == MessageType.MapChanged)
            {
                int j = msg.ReadInt32();
                int i = msg.ReadInt32();
                int tile = msg.ReadInt32();

                _map.Layer2[j, i] = tile;
            }
            
        }

        NetOutgoingMessage CreateMessage(MessageType type)
        {
            NetOutgoingMessage msg = _client.CreateMessage();
            msg.Write((int)type);
            return msg;
        }

        void SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method, int sequenceChannel)
        {
            _client.SendMessage(msg, method, sequenceChannel);
        }

        public void SendPlayerMove()
        {
            NetOutgoingMessage msg = CreateMessage(MessageType.PlayerMoved);

            msg.Write(_player.GetCoordinates().X);
            msg.Write(_player.GetCoordinates().Y);
            msg.Write((int)_player.GetDirection());

            SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 2);
        }

        public void SendMapChange(int j, int i)
        {
            NetOutgoingMessage msg = CreateMessage(MessageType.MapChanged);

            msg.Write(j);
            msg.Write(i);
            msg.Write(_map.Layer2[j, i]);

            SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 3);
        }
    }
}
