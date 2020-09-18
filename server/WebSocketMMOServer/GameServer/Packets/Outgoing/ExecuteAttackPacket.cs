using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer.Packets.Outgoing
{
    public class ExecuteAttackPacket : Packet
    {
        public ExecuteAttackPacket(AttackData data) : base()
        {
            writer.Write((byte)32);

            writer.Write(data.attackerId);
            writer.Write(data.targetId);
        }
    }
}
