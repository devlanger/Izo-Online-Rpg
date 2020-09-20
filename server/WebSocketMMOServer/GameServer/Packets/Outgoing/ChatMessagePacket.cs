using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer.Packets.Outgoing
{
    public class ChatMessagePacket : Packet
    {
        public ChatMessagePacket(string message) : base()
        {
            writer.Write((byte)39);

            writer.Write(message);
        }
    }
}
