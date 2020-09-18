using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer.Packets.Outgoing
{
    public class ExecuteUseSkillPacket : Packet
    {
        public ExecuteUseSkillPacket(int userId, int targetId, int skillId) : base()
        {
            writer.Write((byte)37);

            writer.Write(userId);
            writer.Write(targetId);
            writer.Write(skillId);

        }
    }
}
