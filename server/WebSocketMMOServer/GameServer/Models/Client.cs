using System;
using System.Collections.Generic;
using System.Text;
using WebSocketMMOServer.GameServer;
using WebSocketMMOServer.GameServer.Packets.Outgoing;

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

        public void BindEvents()
        {
            StatsContainer container = ServerManager.Instance.StatsManager.GetContainerForCharacter(SelectedCharacter.Id);
            container.OnStatChanged += Container_OnStatChanged;
        }

        private void Container_OnStatChanged(StatType arg1, object arg2)
        {
            Server.Instance.SendData(ip, new StatSyncPacket(SelectedCharacter.Id, arg1, arg2));
        }
    }
}
