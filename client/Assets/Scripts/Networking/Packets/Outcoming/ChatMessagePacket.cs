using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ChatMessagePacket : Packet
{
    public ChatMessagePacket(string message) : base()
    {
        writer.Write((byte)23);
        writer.Write(message);
    }
}