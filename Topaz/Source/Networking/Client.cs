using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using Topaz.Mob;
using Topaz.World;

namespace Topaz.Networking
{
    public sealed class Client
    {
        NetClient client;
        World.Map map;
        Mob.Player player;
        Dictionary<long, Mob.Player> players;
        string lastMessage;

        private static readonly Lazy<Client> lazy =
            new Lazy<Client>(() => new Client());

        public static Client Instance { get { return lazy.Value; } }

        internal Map Map { get => map; set => map = value; }
        internal Player Player { get => player; set => player = value; }
        internal Dictionary<long, Player> Players { get => players; set => players = value; }

        private Client()
        {
        }

        public void Initialize()
        {
            Console.WriteLine("Starting client...");

            map = new World.Map();
            player = new Mob.Player();
            players = new Dictionary<long, Mob.Player>();

            NetPeerConfiguration config = new NetPeerConfiguration(Properties.Resources.Title);
            client = new NetClient(config);
            client.Start();
            client.Connect(host: "127.0.0.1", port: 12345);
        }

        public void Terminate()
        {
            client.Disconnect("terminating");
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
                Console.Write("From server: ");
                lastMessage = msg.PeekString() + "(" + msg.SenderConnection?.Status + ")";
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        HandleIncomingData(msg);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        Console.WriteLine(msg.SenderConnection.Status);
                        break;
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Console.WriteLine(msg.ReadString());
                        break;
                    default:
                        Console.WriteLine("Unhandled type: " + msg.MessageType);
                        break;
                }
                client.Recycle(msg);
            }
        }

        //public void ClientThread()
        //{
        //    Console.WriteLine("Client started...");
        //    NetIncomingMessage msg;
        //    while (Engine.Window.Instance.State == Engine.Window.WindowState.Running)
        //    {
        //        Thread.Sleep(10);
        //        Console.WriteLine(client);
        //        while ((msg = client.ReadMessage()) != null)
        //        {
        //            Console.Write("From server: ");
        //            switch (msg.MessageType)
        //            {
        //                case NetIncomingMessageType.Data:
        //                    HandleIncomingData(msg);
        //                    break;
        //                case NetIncomingMessageType.StatusChanged:
        //                    Console.WriteLine(msg.SenderConnection.Status);
        //                    break;
        //                case NetIncomingMessageType.VerboseDebugMessage:
        //                case NetIncomingMessageType.DebugMessage:
        //                case NetIncomingMessageType.WarningMessage:
        //                case NetIncomingMessageType.ErrorMessage:
        //                    Console.WriteLine(msg.ReadString());
        //                    break;
        //                default:
        //                    Console.WriteLine("Unhandled type: " + msg.MessageType);
        //                    break;
        //            }
        //            client.Recycle(msg);
        //        }
        //    }
        //    Console.WriteLine("Client terminated...");
        //}

        public void HandleIncomingData(NetIncomingMessage msg)
        {
            string type = msg.ReadString();

            if (type == "SRV_MAP")
            {
                int len0 = msg.ReadInt32();
                int len1 = msg.ReadInt32();
                map.Map1 = new int[len0, len1];
                map.Map2 = new int[len0, len1];
                for (int j = 0; j < map.Map1.GetLength(0); j++)
                {
                    for (int i = 0; i < map.Map1.GetLength(1); i++)
                    {
                        map.Map1[j, i] = msg.ReadInt32();
                    }
                }
                for (int j = 0; j < map.Map2.GetLength(0); j++)
                {
                    for (int i = 0; i < map.Map2.GetLength(1); i++)
                    {
                        map.Map2[j, i] = msg.ReadInt32();
                    }
                }
            }

            if (type == "SRV_NEW_PLAYER")
            {
                long id = msg.ReadInt64();
                float x = msg.ReadFloat();
                float y = msg.ReadFloat();

                players.Add(id, new Player());
            }

            if (type == "SRV_PLAYER_MOVE")
            {
                long id = msg.ReadInt64();
                float x = msg.ReadFloat();
                float y = msg.ReadFloat();

                if (players.ContainsKey(id))
                {
                    Console.WriteLine("C RECV! " + x + "-" + y);
                    players[id].Position = new Vector2(x, y);
                }
            }

            if (type == "SRV_MAP_CHANGE")
            {
                int j = msg.ReadInt32();
                int i = msg.ReadInt32();
                int tile = msg.ReadInt32();

                map.Map2[j, i] = tile;
            }
            
        }

        public void SendPlayerMove()
        {
            NetOutgoingMessage nmsg = client.CreateMessage();
            nmsg.Write("CLT_PLAYER_MOVE");
            nmsg.Write(player.Position.X);
            nmsg.Write(player.Position.Y);
            Console.WriteLine("C SEND! " + player.Position.X + "-" + player.Position.X);
            client.SendMessage(nmsg, NetDeliveryMethod.ReliableOrdered, 2);
        }

        public void SendMapChange(int j, int i)
        {
            NetOutgoingMessage nmsg = client.CreateMessage();
            nmsg.Write("CLT_MAP_CHANGE");
            nmsg.Write(j);
            nmsg.Write(i);
            nmsg.Write(map.Map2[j, i]);
            client.SendMessage(nmsg, NetDeliveryMethod.ReliableOrdered, 3);
        }
    }
}
