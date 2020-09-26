using System;
using System.Collections.Generic;
using System.Text;
using WebSocketMMOServer.GameServer.Packets.Outgoing;

namespace WebSocketMMOServer.GameServer
{
    public class Character
    {
        public int Id { get; set; }
        public virtual int BaseId { get; set; } = 0;
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
        public ClickType OnClick { get; internal set; }

        public Character(int id)
        {
            this.Id = id;
        }

        public virtual void Init()
        {
            BindEvents();
        }

        public void AddStatInt(StatType stat, int goldReward)
        {
            var stats = GetStatsContainer();

            int gold = (int)stats.GetStat(stat).value;
            stats.SetStat(stat, (int)(gold + goldReward));
        }

        public object GetStat(StatType stat)
        {
            return ServerManager.Instance.StatsManager.GetStat(Id, stat);
        }

        public StatsContainer GetStatsContainer()
        {
            return ServerManager.Instance.StatsManager.GetContainerForCharacter(Id);
        }

        public ItemsContainer GetItemsContainer(ItemsContainerId id)
        {
            return ServerManager.Instance.ItemsManager.GetContainer(id, Id);
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

        public virtual void BindEvents()
        {
            GetStatsContainer().OnStatChanged += Character_OnStatChanged;
        }

        private void Character_OnStatChanged(StatType arg1, object arg2)
        {
            switch(arg1)
            {
                case StatType.HEALTH:
                case StatType.MAX_HEALTH:
                    foreach (var item in ServerManager.Instance.CharactersManager.GetClientsInRange(Position))
                    {
                        Server.Instance.SendData(item.Value.ip, new StatSyncPacket(Id, arg1, (int)arg2));
                    }
                    break;
            }
        }
    }

    public enum ClickType : byte
    {
        MOB = 0,
        SHOP = 1,
        NPC = 2
    }
}
