using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer
{
    public enum ItemsContainerId : byte
    {
        INVENTORY = 0,
        WAREHOUSE = 1,
        EQUIPMENT = 2,
        SHOP = 3,
    }

    public class ItemsContainer
    {
        public int Id { get; set; }
        public ItemsContainerId InventoryId { get; set; }
        public Dictionary<int, ItemData> Items { get; private set; } = new Dictionary<int, ItemData>();

        public event Action<ItemsContainer> InventoryChanged = delegate { };
        public int InventorySpace = 16;

        public ItemsContainer(ItemsContainerId containerId, int id)
        {
            this.InventoryId = containerId;
            Id = id;
        }

        public void Refresh()
        {
            InventoryChanged(this);
        }

        public int GetFreeSlot()
        {
            for (int i = 0; i < InventorySpace; i++)
            {
                if(!Items.ContainsKey(i))
                {
                    return i;
                }
            }

            return -1;
        }

        public void AddItem(int slot, ItemData item, bool ignoreEvents = false)
        {
            if (!Items.ContainsKey(slot))
            {
                Items[slot] = item;
                InventoryChanged(this);
            }
        }

        public void RemoveItem(int slot, bool ignoreEvents = false)
        {
            if (Items.ContainsKey(slot))
            {
                Items.Remove(slot);
                if (!ignoreEvents)
                {
                    InventoryChanged(this);
                }
            }
        }
    }
}
