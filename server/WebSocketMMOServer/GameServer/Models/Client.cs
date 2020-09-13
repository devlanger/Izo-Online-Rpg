using System;
using System.Collections.Generic;
using System.Text;
using WebSocketMMOServer.GameServer;

namespace WebSocketMMOServer
{
    public class Client
    {
        public int accountId = -1;
        public string ip;

        public Character SelectedCharacter { get; set; }

        public bool LoggedIn
        {
            get
            {
                return accountId != -1;
            }
        }

        public bool InGameWorld
        {
            get
            {
                return SelectedCharacter != null;
            }
        }
    }
}
