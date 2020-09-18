using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer
{
    public class AttackData
    {
        public int attackerId;
        public int targetId;
        public ushort damage;
        public byte damageType;
    }
}
