using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer.Packets.Outgoing
{
    public class SyncInventoryPacket : Packet
    {
        public SyncInventoryPacket(ItemsContainer items) : base()
        {
            writer.Write((byte)36);

            writer.Write((byte)items.InventoryId);
            writer.Write((ushort)items.Items.Count);
            foreach (var item in items.Items)
            {
                writer.Write((ushort)item.Key);
                writer.Write(item.Value.baseId);
            }
        }
    }
}
