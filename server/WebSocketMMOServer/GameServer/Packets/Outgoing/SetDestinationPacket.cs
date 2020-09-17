using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer.Packets.Outgoing
{
    public class SetDestinationPacket : Packet
    {
        public SetDestinationPacket(Character character) : base()
        {
            writer.Write((byte)33);

            StatsContainer stats = character.GetStatsContainer();
            writer.Write((int)character.Id);
            writer.Write((short)stats.GetStat(StatType.POS_X).value);
            writer.Write((short)stats.GetStat(StatType.POS_Z).value);
        }
    }
}