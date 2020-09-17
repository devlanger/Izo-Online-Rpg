using System;
using System.Collections.Generic;
using System.Text;
using WebSocketMMOServer.GameServer.Models;

namespace WebSocketMMOServer.GameServer.Packets.Outgoing
{
    public class StatSyncPacket : Packet
    {
        public StatSyncPacket(int id, StatType stat, object val) : base()
        {
            writer.Write((byte)35);

            writer.Write(id);
            writer.Write((byte)stat);
            
            switch(stat)
            {
                case StatType.HEALTH:
                case StatType.MANA:
                case StatType.EXPERIENCE:
                case StatType.GOLD:
                    writer.Write((int)val);
                    break;
                case StatType.LEVEL:
                    writer.Write((short)val);
                    break;
            }
        }
    }
}
