using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer
{
    public class Player : Character
    {
        public int DatabaseId { get; set; }

        public Player(int id) : base(id)
        {
        }
    }
}
