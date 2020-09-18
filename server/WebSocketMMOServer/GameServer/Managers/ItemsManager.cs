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

        public void AddInventoryForCharacter(Player character)
        {
            if (inventoryContainers.ContainsKey(character.Id))
            {
                Console.WriteLine("Trying to duplicate stats container for id: " + character.Id);
                return;
            }

            ItemsContainer container = new ItemsContainer()
            {
                Id = character.Id
            };

            inventoryContainers.Add(character.Id, container);
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
