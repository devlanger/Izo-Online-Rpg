using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer.Packets.Outgoing
{
    public class CharacterSelectionPacket : Packet
    {
        public CharacterSelectionPacket() : base()
        {
            writer.Write((byte)2);
        }
    }
}
