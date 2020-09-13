using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using WatsonWebsocket;
using WebSocketMMOServer.Database;

namespace WebSocketMMOServer.GameServer
{
    class Server
    {
        public static Server Instance { get; private set; }

        private WatsonWsServer server;
        private ServerManager manager;

        public void Start()
        {
            Instance = this;
            manager = new ServerManager();

            server = new WatsonWsServer("127.0.0.1", 3001, false);
            server.ClientConnected += ClientConnected;
            server.ClientDisconnected += ClientDisconnected;
            server.MessageReceived += MessageReceived;
            server.Start();
        }

        public void ClientConnected(object sender, ClientConnectedEventArgs args)
        {
            Console.WriteLine("Client connected: " + args.IpPort);
            manager.ClientsManager.clients.Add(args.IpPort, new Client()
            {
                ip = args.IpPort
            });
        }

        public void ClientDisconnected(object sender, ClientDisconnectedEventArgs args)
        {
            Console.WriteLine("Client disconnected: " + args.IpPort);
            Client client = manager.ClientsManager.clients[args.IpPort];

            if (client.SelectedCharacter != null)
            {
                ServerManager.Instance.CharactersManager.DespawnCharacter(client.SelectedCharacter);

                client.SelectedCharacter = null;
            }
            manager.ClientsManager.clients.Remove(args.IpPort);
        }

        public void MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            MemoryStream stream = new MemoryStream(args.Data);
            BinaryReader reader = new BinaryReader(stream);

            while (stream.Position < reader.BaseStream.Length)
            {
                byte packetType = reader.ReadByte();
                GamePacketType packet = (GamePacketType)packetType;
                GamePacketsImpl.ExecutePacket(packet, manager.ClientsManager.clients[args.IpPort], reader);
            }
        }

        public void SendData(string ip, byte[] data)
        {
            server.SendAsync(ip, data);
        }

        public void SendData(string ip, Packet packet)
        {
            server.SendAsync(ip, packet.Data);
        }
    }
}
