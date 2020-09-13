using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer.Packets
{
    public class DespawnCharacterPacket : Packet
    {
        public DespawnCharacterPacket(Character character) : base()
        {
            writer.Write((byte)31);
            writer.Write(character.Id);
        }

        public DespawnCharacterPacket(int id) : base()
        {
            writer.Write((byte)31);
            writer.Write(id);
        }
    }
}
