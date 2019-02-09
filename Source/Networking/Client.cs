using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Topaz.Entity;
using Topaz.World;

namespace Topaz.Networking
{
    public sealed class Client
    {
        private Engine.Logger _logger = new Engine.Logger("Client");

        private NetClient _client;
        private long _localId;

        internal Chunk Map { get; set; }
        internal Player Player { get; set; }
        internal Dictionary<long, Player> Players { get; set; }
        public int LastLatency { get; set; }
        public string LastNetMessage { get; set; }

        private static readonly Lazy<Client> lazy =
            new Lazy<Client>(() => new Client());

        public static Client Instance { get { return lazy.Value; } }

        private Client()
        {
        }

        public void Initialize()
        {
            _logger.Info("Starting...");

            Map = new World.Chunk();
            Player = new Entity.Player();
            Players = new Dictionary<long, Entity.Player>();

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
                    LastNetMessage = msg.PeekString();

                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        _logger.Info("StatusChanged received: " + msg.SenderConnection.Status);
                        break;
                    case NetIncomingMessageType.Data:
                        LastNetMessage = (MessageType)msg.PeekInt32() + "";
                        HandleIncomingData(msg);
                        break;
                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                        LastLatency = (int)Math.Round(msg.ReadFloat()*1000);
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
                Players.Add(_localId, new Player());
                Players[_localId].LoadContent();
                Players[_localId].DrawAtOrigin = true;
                Player = Players[_localId];
            }

            if (type == MessageType.PlayerDisconnected)
            {
                long id = msg.ReadInt64();
                Players.Remove(id);
            }

            if (type == MessageType.MapInfo)
            {
                int rows = msg.ReadInt32();
                int cols = msg.ReadInt32();

                Map.Layer1 = new int[rows, cols];
                Map.Layer2 = new int[rows, cols];

                for (int j = 0; j < rows; j++)
                    for (int i = 0; i < cols; i++)
                        Map.Layer1[j, i] = msg.ReadInt32();

                for (int j = 0; j < rows; j++)
                    for (int i = 0; i < cols; i++)
                        Map.Layer2[j, i] = msg.ReadInt32();
            }

            if (type == MessageType.PlayerConnected)
            {
                long id = msg.ReadInt64();
                float x = msg.ReadFloat();
                float y = msg.ReadFloat();
                Entity.Entity.Direction direction = (Entity.Entity.Direction)msg.ReadInt32();

                Players.Add(id, new Player());
                Players[id].LoadContent();
                Players[id].SetCoordinates(new Vector2(x, y));
                Players[id].SetDirection(direction);
            }

            if (type == MessageType.PlayerMoved)
            {
                long id = msg.ReadInt64();
                float x = msg.ReadFloat();
                float y = msg.ReadFloat();
                Entity.Entity.Direction direction = (Entity.Entity.Direction)msg.ReadInt32();

                if (Players.ContainsKey(id))
                {
                    if (id == _localId && direction == Entity.Entity.Direction.None)
                    {
                        Players[id].SetCoordinates(new Vector2(x, y));
                    }
                    else if (id != _localId)
                    {
                        Players[id].SetCoordinates(new Vector2(x, y));
                        Players[id].SetDirection(direction);
                    }
                    
                }
            }

            if (type == MessageType.MapChanged)
            {
                int j = msg.ReadInt32();
                int i = msg.ReadInt32();
                int tile = msg.ReadInt32();

                Map.Layer2[j, i] = tile;
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

            msg.Write(Player.Coordinates.X);
            msg.Write(Player.Coordinates.Y);
            msg.Write((int)Player.MovementDirection);

            SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 2);
        }

        public void SendMapChange(int j, int i)
        {
            NetOutgoingMessage msg = CreateMessage(MessageType.MapChanged);

            msg.Write(j);
            msg.Write(i);
            msg.Write(Map.Layer2[j, i]);

            SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 3);
        }
    }
}
