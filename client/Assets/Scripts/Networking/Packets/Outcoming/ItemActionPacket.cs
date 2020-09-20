using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ItemActionPacket : Packet
{
    public ItemActionPacket(ItemActionData action) : base()
    {
        writer.Write((byte)22);
        writer.Write((byte)action.action);
        writer.Write((byte)action.sourceContainer);
        writer.Write((byte)action.targetContainer);
        writer.Write(action.sourceSlot);
        writer.Write(action.targetSlot);
    }
}

public class ItemActionData
{
    public ItemAction action;

    public ItemContainerId sourceContainer;
    public ItemContainerId targetContainer;

    public int sourceSlot;
    public int targetSlot;
}

public enum ItemAction
{
    MOVE = 1,
    DELETE = 2,
    USE = 3,
}
