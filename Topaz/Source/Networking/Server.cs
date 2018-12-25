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

            NetPeerConfiguration config = new NetPeerConfiguration(Properties.Resources.Title);
            config.Port = 12345;
            config.EnableUPnP = true;
            config.ConnectionTimeout = 10;

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
                        case NetIncomingMessageType.StatusChanged:
                            HandleStatusChanged(msg);
                            break;
                        case NetIncomingMessageType.Data:
                            HandleIncomingData(msg);
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

        public void HandleStatusChanged(NetIncomingMessage msg)
        {
            if (msg.SenderConnection.Status == NetConnectionStatus.Connected)
            {
                connections.Add(msg.SenderConnection.RemoteUniqueIdentifier, new Networking.Connection(msg.SenderConnection));
                SendConnectionInfo(msg.SenderConnection);
                SendMap(msg.SenderConnection);
                SendPlayerInfo(connections[msg.SenderConnection.RemoteUniqueIdentifier]);
            }

            if (msg.SenderConnection.Status == NetConnectionStatus.Disconnected)
            {
                connections.Remove(msg.SenderConnection.RemoteUniqueIdentifier);
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

                connections[msg.SenderConnection.RemoteUniqueIdentifier].Player.Position = new Vector2(x, y);

                SendPlayerMove(msg.SenderConnection.RemoteUniqueIdentifier, x, y);
            }

            if (type == MessageType.MapChanged)
            {
                int j = msg.ReadInt32();
                int i = msg.ReadInt32();
                int tile = msg.ReadInt32();

                map.Map2[j, i] = tile;

                SendMapChange(j, i, tile);
            }
        }

        public NetOutgoingMessage CreateMessage(MessageType type)
        {
            NetOutgoingMessage msg = server.CreateMessage();
            msg.Write((int)type);
            return msg;
        }

        public void SendMessage(NetOutgoingMessage msg, NetConnection recipient, NetDeliveryMethod method, int sequenceChannel)
        {
            server.SendMessage(msg, recipient, method, sequenceChannel);
        }

        public void SendMessage(NetOutgoingMessage msg, List<NetConnection> recipients, NetDeliveryMethod method, int sequenceChannel)
        {
            if (recipients.Count == 0)
                return;

            server.SendMessage(msg, recipients, method, sequenceChannel);
        }

        public void SendConnectionInfo(NetConnection connection)
        {
            NetOutgoingMessage msg = CreateMessage(MessageType.ConnectionInfo);

            msg.Write(connection.RemoteUniqueIdentifier);

            server.SendMessage(msg, connection, NetDeliveryMethod.ReliableOrdered, 1);
        }

        public void SendMap(NetConnection connection)
        {
            NetOutgoingMessage msg = CreateMessage(MessageType.MapInfo);

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
            List<NetConnection> recipients = new List<NetConnection>();

            foreach (long id in connections.Keys)
            {
                if (id != connection.NetConnection.RemoteUniqueIdentifier)
                {
                    recipients.Add(connections[id].NetConnection);

                    // Send existing players to newly connected player
                    NetOutgoingMessage msg = CreateMessage(MessageType.PlayerConnected);

                    msg.Write(id);
                    msg.Write(connections[id].Player.Position.X);
                    msg.Write(connections[id].Player.Position.Y);

                    SendMessage(msg, connection.NetConnection, NetDeliveryMethod.ReliableOrdered, 1);
                }
            }

            // Send newly connected player to other players.
            {
                NetOutgoingMessage msg = CreateMessage(MessageType.PlayerConnected);

                msg.Write(connection.NetConnection.RemoteUniqueIdentifier);
                msg.Write(connection.Player.Position.X);
                msg.Write(connection.Player.Position.Y);

                SendMessage(msg, recipients, NetDeliveryMethod.ReliableOrdered, 1);
            }
        }

        public void SendPlayerDisconnected(NetConnection connection)
        {
            List<NetConnection> recipients = new List<NetConnection>();
            foreach (long id in connections.Keys)
                recipients.Add(connections[id].NetConnection);

            foreach (long id in connections.Keys)
            {
                NetOutgoingMessage msg = CreateMessage(MessageType.PlayerDisconnected);

                msg.Write(connection.RemoteUniqueIdentifier);

                SendMessage(msg, recipients, NetDeliveryMethod.ReliableOrdered, 2);
            }
        }

        public void SendPlayerMove(long srcId, float x, float y)
        {
            List<NetConnection> recipients = new List<NetConnection>();
            foreach (long id in connections.Keys)
                recipients.Add(connections[id].NetConnection);

            NetOutgoingMessage nmsg = CreateMessage(MessageType.PlayerMoved);
            nmsg.Write(srcId);
            nmsg.Write(x);
            nmsg.Write(y);

            server.SendMessage(nmsg, recipients, NetDeliveryMethod.ReliableOrdered, 2);
        }

        public void SendMapChange(int j, int i, int tile)
        {
            List<NetConnection> recipients = new List<NetConnection>();
            foreach (long id in connections.Keys)
                recipients.Add(connections[id].NetConnection);
            
            NetOutgoingMessage nmsg = CreateMessage(MessageType.MapChanged);

            nmsg.Write(j);
            nmsg.Write(i);
            nmsg.Write(map.Map2[j, i]);

            server.SendMessage(nmsg, recipients, NetDeliveryMethod.ReliableOrdered, 2);
        }
    }
}
