using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WatsonWebsocket;
using WebSocketMMOServer.GameServer;

namespace WebSocketMMOServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();

            while (true)
            {
                ServerManager.Instance.TickManager.Tick();
                Thread.Sleep(100);
            }
        }
    }
}
