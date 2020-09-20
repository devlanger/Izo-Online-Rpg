using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using WebSocketMMOServer.Database;

namespace WebSocketMMOServer.GameServer
{
    public class ItemsManager
    {
        private Dictionary<int, Dictionary<ItemsContainerId, ItemsContainer>> inventoryContainers = new Dictionary<int, Dictionary<ItemsContainerId, ItemsContainer>>();
        public int lastItemId = 1;

        public ItemsManager()
        {
            lastItemId = DatabaseManager.GetLastInsertedId("items");
        }

        public ItemsContainer GetContainer(ItemsContainerId id, int containerId)
        {
            if(inventoryContainers.ContainsKey(containerId))
            {
                return inventoryContainers[containerId][id];
            }

            return null;
        }

        public Dictionary<ItemsContainerId, ItemsContainer> GetContainers(int containerId)
        {
            if (inventoryContainers.ContainsKey(containerId))
            {
                return inventoryContainers[containerId];
            }

            return null;
        }

        public void AddInventoryForCharacter(Player character)
        {
            if (inventoryContainers.ContainsKey(character.Id))
            {
                Console.WriteLine("Trying to duplicate stats container for id: " + character.Id);
                return;
            }

            inventoryContainers.Add(character.Id, new Dictionary<ItemsContainerId, ItemsContainer>()
            {
                { ItemsContainerId.INVENTORY, new ItemsContainer(ItemsContainerId.INVENTORY,character.Id) },
                { ItemsContainerId.WAREHOUSE, new ItemsContainer(ItemsContainerId.WAREHOUSE,character.Id) },
                { ItemsContainerId.EQUIPMENT, new ItemsContainer(ItemsContainerId.EQUIPMENT,character.Id) },
                { ItemsContainerId.SHOP, new ItemsContainer(ItemsContainerId.SHOP,character.Id) },
            });
        }

        public void RemoveInventoryForCharacter(int id)
        {
            if (inventoryContainers.ContainsKey(id))
            {
                inventoryContainers.Remove(id);
            }
        }
    }
}
