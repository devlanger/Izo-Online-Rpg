using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace WebSocketMMOServer.GameServer
{
    public class Mob : Character
    {
        public short StartPosX, StartPosZ;

        public Mob(int id) : base(id)
        {

        }

        public void SetSpawnPosition(short posX, short posZ)
        {
            StartPosX = posX;
            StartPosZ = posZ;
        }

    }
}
