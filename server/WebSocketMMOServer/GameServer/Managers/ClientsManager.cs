using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer
{
    public class ClientsManager
    {
        public Dictionary<string, Client> clients = new Dictionary<string, Client>();
    }
}
