using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer
{
    public class ItemsContainer
    {
        public int Id { get; set; }
        public Dictionary<int, ItemData> Items { get; private set; } = new Dictionary<int, ItemData>();

        public event Action InventoryChanged = delegate { };
        public int InventorySpace = 16;

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

        public void AddItem(int slot, ItemData item)
        {
            if (!Items.ContainsKey(slot))
            {
                Items[slot] = item;
                InventoryChanged();
            }
        }

        public void RemoveItem(int slot)
        {
            if (Items.ContainsKey(slot))
            {
                Items.Remove(slot);
                InventoryChanged();
            }
        }
    }
}
