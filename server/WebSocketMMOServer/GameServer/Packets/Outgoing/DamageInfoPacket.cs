using System;
using System.Collections.Generic;
using System.Text;
using WebSocketMMOServer.GameServer.Models;

namespace WebSocketMMOServer.GameServer.Packets.Outgoing
{
    public class DamageInfoPacket : Packet
    {
        public DamageInfoPacket(AttackData data) : base()
        {
            writer.Write((byte)34);

            writer.Write(data.targetId);
            writer.Write(data.damage);
            writer.Write(data.damageType);
        }
    }
}
