using System;
using System.Collections.Generic;
using System.Text;
using WebSocketMMOServer.GameServer.Packets.Outgoing;

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

        public void SetDestination(short posX, short posZ)
        {
            StatsContainer container = GetStatsContainer();
            container.SetStat(StatType.POS_X, (short)posX);
            container.SetStat(StatType.POS_Z, (short)posZ);

            foreach (var item in ServerManager.Instance.CharactersManager.GetClientsInRange(Position, 50, Id))
            {
                Server.Instance.SendData(item.Value.ip, new SetDestinationPacket(this));
            }
        }

        public void SnapToPosition(short posX, short posZ)
        {
            StatsContainer container = GetStatsContainer();
            container.SetStat(StatType.POS_X, (short)posX);
            container.SetStat(StatType.POS_Z, (short)posZ);

            foreach (var item in ServerManager.Instance.CharactersManager.GetClientsInRange(Position, 50))
            {
                Server.Instance.SendData(item.Value.ip, new SnapPositionPacket(this));
            }
        }
    }
}
