using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace WebSocketMMOServer.GameServer
{
    public class KingdomsManager
    {
        public class KingdomData
        {
            public int id;
            public Vector3 spawnPoint;
        }

        public static Dictionary<int, KingdomData> kingdoms = new Dictionary<int, KingdomData>()
        {
            { 0, new KingdomData()
            {
                id = 0,
                spawnPoint = new Vector3(70, 0, 105)
            } },
            { 1, new KingdomData()
            {
                id = 1,
                spawnPoint = new Vector3(820, 0, 833)
            } },
        };
    }
}
