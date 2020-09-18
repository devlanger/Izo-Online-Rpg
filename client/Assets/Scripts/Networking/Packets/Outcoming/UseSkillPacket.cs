using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UseSkillPacket : Packet
{
    public UseSkillPacket(int skillId) : base()
    {
        writer.Write((byte)21);
        writer.Write(skillId);
    }
}