using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer.Packets
{
    public class RequestItemsList : Packet
    {
        public RequestItemsList(int containerId) : base()
        {
            ItemsContainer container = ServerManager.Instance.ItemsManager.GetContainer(containerId);

            foreach (var item in container.items)
            {
                writer.Write(item.Key);
                writer.Write(item.Value.uniqueId);
                writer.Write(item.Value.baseId);
            }
        }
    }
}
