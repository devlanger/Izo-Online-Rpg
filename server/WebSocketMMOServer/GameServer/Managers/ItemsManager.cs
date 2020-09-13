using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer
{
    public class ItemsManager
    {
        private Dictionary<int, ItemsContainer> inventoryContainers = new Dictionary<int, ItemsContainer>();

        public ItemsContainer GetContainer(int containerId)
        {
            if(inventoryContainers.ContainsKey(containerId))
            {
                return inventoryContainers[containerId];
            }

            return null;
        }
    }
}
