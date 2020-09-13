using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer
{
    public class ItemsContainer
    {
        public int Id { get; set; }
        public Dictionary<int, ItemData> items = new Dictionary<int, ItemData>();
    }
}
