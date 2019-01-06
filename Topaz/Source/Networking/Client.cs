﻿using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Topaz.Mob;
using Topaz.World;

namespace Topaz.Networking
{
    public sealed class Client
    {
        Engine.Logger logger = new Engine.Logger("Client");

        NetClient client;
        World.Map map;
        Mob.Player player;
        Dictionary<long, Mob.Player> players;
        string lastMessage;
        int lastLatency;
        long localId;

        private static readonly Lazy<Client> lazy =
            new Lazy<Client>(() => new Client());

        public static Client Instance { get { return lazy.Value; } }

        internal Map Map { get => map; set => map = value; }
        internal Player Player { get => player; set => player = value; }
        internal Dictionary<long, Player> Players { get => players; set => players = value; }
        public int LastLatency { get => lastLatency; set => lastLatency = value; }

        private Client()
        {
        }

        public void Initialize()
        {
            logger.Info("Starting...");

            map = new World.Map();
            player = new Mob.Player();
            players = new Dictionary<long, Mob.Player>();

            NetPeerConfiguration config = new NetPeerConfiguration(Properties.Resources.Title);
            config.ConnectionTimeout = 10;
            config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);

            client = new NetClient(config);
            client.Start();

            logger.Info("Started.");
        }

        public void Connect(string host, int port)
        {
            client.Connect(host: host, port: port);
        }

        public void Disconnect()
        {
            client.Disconnect("terminating");
            logger.Info("Disconnected.");
        }

        public NetConnectionStatus GetClientConnectionStatus()
        {
            return client.ConnectionStatus;
        }

        public string GetLastMessage()
        {
            return lastMessage;
        }

        public void HandleMessages()
        {
            NetIncomingMessage msg;
            while ((msg = client.ReadMessage()) != null)
            {
                lastMessage = msg.PeekString() + "(" + msg.SenderConnection?.Status + ")";

                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        logger.Info("StatusChanged received: " + msg.SenderConnection.Status);
                        break;
                    case NetIncomingMessageType.Data:
                        lastMessage = (MessageType)msg.PeekInt32() + "(" + msg.SenderConnection?.Status + ")";
                        HandleIncomingData(msg);
                        break;
                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                        lastLatency = (int)Math.Round(msg.ReadFloat()*1000);
                        break;
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                        logger.Debug(msg.ReadString());
                        break;
                    case NetIncomingMessageType.WarningMessage:
                        logger.Warn(msg.ReadString());
                        break;
                    case NetIncomingMessageType.ErrorMessage:
                        logger.Error(msg.ReadString());
                        break;
                    default:
                        logger.Warn("Unhandled MessageType: " + msg.MessageType);
                        break;
                }

                client.Recycle(msg);
            }
        }

        public void HandleIncomingData(NetIncomingMessage msg)
        {
            MessageType type = (MessageType)msg.ReadInt32();

            if (type == MessageType.ConnectionInfo)
            {
                localId = msg.ReadInt64();
                players.Add(localId, new Player());
                players[localId].LoadContent();
                players[localId].DrawAtOrigin = true;
                player = players[localId];
            }

            if (type == MessageType.PlayerDisconnected)
            {
                long id = msg.ReadInt64();
                players.Remove(id);
            }

            if (type == MessageType.MapInfo)
            {
                int rows = msg.ReadInt32();
                int cols = msg.ReadInt32();

                map.Map1 = new int[rows, cols];
                map.Map2 = new int[rows, cols];

                for (int j = 0; j < rows; j++)
                    for (int i = 0; i < cols; i++)
                        map.Map1[j, i] = msg.ReadInt32();

                for (int j = 0; j < rows; j++)
                    for (int i = 0; i < cols; i++)
                        map.Map2[j, i] = msg.ReadInt32();
            }

            if (type == MessageType.PlayerConnected)
            {
                long id = msg.ReadInt64();
                float x = msg.ReadFloat();
                float y = msg.ReadFloat();
                Mob.Mob.Direction direction = (Mob.Mob.Direction)msg.ReadInt32();

                players.Add(id, new Player());
                players[id].LoadContent();
                players[id].SetPosition(new Vector2(x, y));
                players[id].SetDirection(direction);
            }

            if (type == MessageType.PlayerMoved)
            {
                long id = msg.ReadInt64();
                float x = msg.ReadFloat();
                float y = msg.ReadFloat();
                Mob.Mob.Direction direction = (Mob.Mob.Direction)msg.ReadInt32();

                if (players.ContainsKey(id))
                {
                    if (id == localId && direction == Mob.Mob.Direction.None)
                    {
                        players[id].SetPosition(new Vector2(x, y));
                    }
                    else if (id != localId)
                    {
                        players[id].SetPosition(new Vector2(x, y));
                        players[id].SetDirection(direction);
                    }
                    
                }
            }

            if (type == MessageType.MapChanged)
            {
                int j = msg.ReadInt32();
                int i = msg.ReadInt32();
                int tile = msg.ReadInt32();

                map.Map2[j, i] = tile;
            }
            
        }

        public NetOutgoingMessage CreateMessage(MessageType type)
        {
            NetOutgoingMessage msg = client.CreateMessage();
            msg.Write((int)type);
            return msg;
        }

        public void SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method, int sequenceChannel)
        {
            client.SendMessage(msg, method, sequenceChannel);
        }

        public void SendPlayerMove()
        {
            NetOutgoingMessage msg = CreateMessage(MessageType.PlayerMoved);

            msg.Write(player.GetPosition().X);
            msg.Write(player.GetPosition().Y);
            msg.Write((int)player.GetDirection());

            SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 2);
        }

        public void SendMapChange(int j, int i)
        {
            NetOutgoingMessage msg = CreateMessage(MessageType.MapChanged);

            msg.Write(j);
            msg.Write(i);
            msg.Write(map.Map2[j, i]);

            SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 3);
        }
    }
}
