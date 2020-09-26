using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer.Packets.Outgoing
{
    public class SpawnCharacterPacket : Packet
    {
        public SpawnCharacterPacket(Character character) : base()
        {
            writer.Write((byte)30);

            StatsContainer stats = character.GetStatsContainer();
            writer.Write(character.Id);
            writer.Write(character.BaseId);
            writer.Write((string)stats.GetStat(StatType.NAME).value);
            writer.Write((short)stats.GetStat(StatType.POS_X).value);
            writer.Write((short)stats.GetStat(StatType.POS_Z).value);
            writer.Write((short)stats.GetStat(StatType.ROTATION).value);
            writer.Write((byte)stats.GetStat(StatType.RACE).value);
            writer.Write((byte)stats.GetStat(StatType.CLASS).value);
            writer.Write((int)stats.GetStat(StatType.HEALTH).value);
            writer.Write((int)stats.GetStat(StatType.MAX_HEALTH).value);
        }
    }
}
