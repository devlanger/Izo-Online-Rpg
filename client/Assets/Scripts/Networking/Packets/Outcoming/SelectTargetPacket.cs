using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SelectTargetPacket : Packet
{
    public SelectTargetPacket(int targetId, byte action) : base()
    {
        writer.Write((byte)12);
        writer.Write(targetId);
        writer.Write(action);
    }
}