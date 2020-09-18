using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer.Packets.Outgoing
{
    public class SnapPositionPacket : Packet
    {
        public SnapPositionPacket(Character character) : base()
        {
            writer.Write((byte)38);

            StatsContainer stats = character.GetStatsContainer();
            writer.Write((int)character.Id);
            writer.Write((short)stats.GetStat(StatType.POS_X).value);
            writer.Write((short)stats.GetStat(StatType.POS_Z).value);
        }
    }
}