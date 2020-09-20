using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer.Packets
{
    public class RequestItemsList : Packet
    {
        public RequestItemsList(Character user, int containerId) : base()
        {
            ItemsContainer container = ServerManager.Instance.ItemsManager.GetContainer((ItemsContainerId)containerId, user.Id);

            foreach (var item in container.Items)
            {
                writer.Write(item.Key);
                writer.Write(item.Value.uniqueId);
                writer.Write(item.Value.baseId);
            }
        }
    }
}
