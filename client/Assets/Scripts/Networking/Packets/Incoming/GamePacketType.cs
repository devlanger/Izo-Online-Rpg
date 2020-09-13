using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer
{
    public enum GamePacketType
    {
        CREATE_CHARACTER = 2,

        SET_DESTINATION = 11,
        SET_ATTACK_TARGET = 12,

        USE_SKILL = 21,
    }
}
