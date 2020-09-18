using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketMMOServer.GameServer
{
    public class Character
    {
        public int Id { get; set; }
        public SelectionState SelectionState { get; set; }
        public HashSet<int> lastSeenCharacters = new HashSet<int>();

        public System.Numerics.Vector3 Position
        {
            get
            {
                short posX = (short)GetStat(StatType.POS_X);
                short posZ = (short)GetStat(StatType.POS_Z);

                return new System.Numerics.Vector3(posX, 0, posZ);
            }
        }

        public bool IsDead
        {
            get
            {
                return (int)GetStat(StatType.HEALTH) <= 0;
            }
        }

        public float LastAttackTime { get; internal set; }

        public Character(int id)
        {
            this.Id = id;
        }

        public object GetStat(StatType stat)
        {
            return ServerManager.Instance.StatsManager.GetStat(Id, stat);
        }

        public StatsContainer GetStatsContainer()
        {
            return ServerManager.Instance.StatsManager.GetContainerForCharacter(Id);
        }

        public ItemsContainer GetInventoryContainer()
        {
            return ServerManager.Instance.ItemsManager.GetContainer(Id);
        }
    }
}
