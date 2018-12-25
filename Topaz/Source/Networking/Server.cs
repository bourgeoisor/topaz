using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Topaz.Networking
{
    public sealed class Server
    {
        Thread thread;
        NetServer server;
        Dictionary<long, Networking.Connection> connections;
        World.Map map;

        private static readonly Lazy<Server> lazy =
            new Lazy<Server>(() => new Server());

        public static Server Instance { get { return lazy.Value; } }

        private Server()
        {
        }

        public void Initialize()
        {
            Console.WriteLine("Starting server...");

            map = new World.Map();
            map.GenerateRandom();
            connections = new Dictionary<long, Networking.Connection>();

            NetPeerConfiguration config = new NetPeerConfiguration(Properties.Resources.Title)
            { Port = 12345, EnableUPnP = true };
            server = new NetServer(config);
            server.Start();

            //server.UPnP.ForwardPort(12345, Properties.Resources.Title);

            thread = new Thread(ServerThread);
            thread.Start();
        }

        public void Terminate()
        {
            if (thread != null)
            {
                server.Shutdown("terminating");
                thread.Abort();
            }
        }

        public void ServerThread()
        {
            Console.WriteLine("Server started...");

            NetIncomingMessage msg;
            while (Engine.Window.Instance.State == Engine.Window.WindowState.Running)
            {
                Thread.Sleep(10);
                while ((msg = server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            HandleIncomingData(msg);
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            if (msg.SenderConnection.Status == NetConnectionStatus.Connected)
                            {
                                connections.Add(msg.SenderConnection.RemoteUniqueIdentifier, new Networking.Connection(msg.SenderConnection));
                                SendMap(msg.SenderConnection);
                                SendPlayerInfo(connections[msg.SenderConnection.RemoteUniqueIdentifier]);
                            }
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
                    server.Recycle(msg);
                }
            }

            Console.WriteLine("Server terminated...");
        }

        public void HandleIncomingData(NetIncomingMessage msg)
        {
            string type = msg.ReadString();

            if (type == "CLT_PLAYER_MOVE")
            {
                float x = msg.ReadFloat();
                float y = msg.ReadFloat();

                Console.WriteLine("S RECV! " + x + "-" + y);

                connections[msg.SenderConnection.RemoteUniqueIdentifier].Player.Position = new Vector2(x, y);

                // @todo: actually verify this is legit

                foreach (long id in connections.Keys)
                {
                    if (id != msg.SenderConnection.RemoteUniqueIdentifier)
                    {
                        // @todo: only create message once, send to multiple
                        NetOutgoingMessage nmsg = server.CreateMessage();
                        nmsg.Write("SRV_PLAYER_MOVE");
                        nmsg.Write(msg.SenderConnection.RemoteUniqueIdentifier);
                        nmsg.Write(x);
                        nmsg.Write(y);

                        Console.WriteLine("S SEND! " + x + "-" + y);

                        server.SendMessage(nmsg, connections[id].NetConnection, NetDeliveryMethod.ReliableOrdered, 2);
                    }
                }
            }

            if (type == "CLT_MAP_CHANGE")
            {
                int j = msg.ReadInt32();
                int i = msg.ReadInt32();
                int tile = msg.ReadInt32();

                map.Map2[j, i] = tile;

                foreach (long id in connections.Keys)
                {
                    // @todo: only create message once, send to multiple
                    NetOutgoingMessage nmsg = server.CreateMessage();
                    nmsg.Write("SRV_MAP_CHANGE");
                    nmsg.Write(j);
                    nmsg.Write(i);
                    nmsg.Write(map.Map2[j, i]);

                    server.SendMessage(nmsg, connections[id].NetConnection, NetDeliveryMethod.ReliableOrdered, 2);
                }
            }
        }

        public void SendMap(NetConnection connection)
        {
            NetOutgoingMessage msg = server.CreateMessage();
            msg.Write("SRV_MAP");
            msg.Write(map.Map1.GetLength(0));
            msg.Write(map.Map1.GetLength(1));
            for (int j = 0; j < map.Map1.GetLength(0); j++)
            {
                for (int i = 0; i < map.Map1.GetLength(1); i++)
                {
                    msg.Write(map.Map1[j, i]);
                }
            }
            for (int j = 0; j < map.Map2.GetLength(0); j++)
            {
                for (int i = 0; i < map.Map2.GetLength(1); i++)
                {
                    msg.Write(map.Map2[j, i]);
                }
            }

            server.SendMessage(msg, connection, NetDeliveryMethod.ReliableOrdered, 1);
        }

        private void SendPlayerInfo(Networking.Connection connection)
        {
            foreach (long id in connections.Keys)
            {
                if (id != connection.NetConnection.RemoteUniqueIdentifier)
                {
                    // send new player to everyone else
                    NetOutgoingMessage msg = server.CreateMessage();
                    msg.Write("SRV_NEW_PLAYER");
                    msg.Write(connection.NetConnection.RemoteUniqueIdentifier);
                    msg.Write(connection.Player.Position.X);
                    msg.Write(connection.Player.Position.Y);
                    server.SendMessage(msg, connections[id].NetConnection, NetDeliveryMethod.ReliableOrdered, 1);

                    // send existing players to new guy
                    NetOutgoingMessage msg2 = server.CreateMessage();
                    msg2.Write("SRV_NEW_PLAYER");
                    msg2.Write(id);
                    msg2.Write(connections[id].Player.Position.X);
                    msg2.Write(connections[id].Player.Position.Y);
                    server.SendMessage(msg2, connection.NetConnection, NetDeliveryMethod.ReliableOrdered, 1);
                }
            } 
        }
    }
}
