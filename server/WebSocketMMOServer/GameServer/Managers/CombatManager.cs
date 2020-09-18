using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using WebSocketMMOServer.GameServer.Packets.Outgoing;

namespace WebSocketMMOServer.GameServer
{
    public class CombatManager
    {
        private CharactersManager charactersManager;
        private TickManager tickManager;

        public CombatManager(TickManager tickManager, CharactersManager charactersManager)
        {
            this.tickManager = tickManager;
            this.charactersManager = charactersManager;

            tickManager.OnTick += TickManager_OnTick;
        }

        private void TickManager_OnTick()
        {
            Dictionary<int, Character> characters = new Dictionary<int, Character>(charactersManager.characters);
            foreach (var character in characters)
            {
                if(character.Value.SelectionState != SelectionState.ATTACK)
                {
                    continue;
                }

                int targetId = (int)character.Value.GetStat(StatType.TARGET_ID);
                ushort damage = 25;
                if (targetId != -1 && targetId != character.Value.Id)
                {
                    if (tickManager.Time > character.Value.LastAttackTime + 1)
                    {
                        if (characters.ContainsKey(targetId))
                        {
                            Character target = characters[targetId];
                            if (Vector3.Distance(character.Value.Position, target.Position) < 4)
                            {
                                AttackData data = new AttackData()
                                {
                                    attackerId = character.Value.Id,
                                    targetId = target.Id,
                                    damage = damage,
                                    damageType = 0
                                };

                                foreach (var item in charactersManager.GetClientsInRange(character.Value.Position, 50))
                                {
                                    Server.Instance.SendData(item.Value.ip, new ExecuteAttackPacket(data));
                                }

                                DealDamage(character.Value, target, data);
                                character.Value.LastAttackTime = tickManager.Time;
                            }
                        }
                    }
                }
            }
        }

        public void DealDamage(Character character, Character target, AttackData attack)
        {
            StatsContainer container = target.GetStatsContainer();

            int health = (int)container.GetStat(StatType.HEALTH).value;
            container.SetStat(StatType.HEALTH, health - attack.damage);

            foreach (var item in charactersManager.GetClientsInRange(character.Position, 50))
            {
                Server.Instance.SendData(item.Value.ip, new DamageInfoPacket(attack));
            }

            if (target.IsDead)
            {
                ServerManager.Instance.CharactersManager.DespawnCharacter(target);

                if (character is Player)
                {
                    StatsContainer killerContainer = character.GetStatsContainer();
                    int exp = (int)killerContainer.GetStat(StatType.EXPERIENCE).value;
                    killerContainer.SetStat(StatType.EXPERIENCE, exp + 25);

                    if ((int)killerContainer.GetStat(StatType.EXPERIENCE).value >= 100)
                    {
                        killerContainer.SetStat(StatType.EXPERIENCE, 0);

                        short level = (short)killerContainer.GetStat(StatType.LEVEL).value;
                        killerContainer.SetStat(StatType.LEVEL, (short)(level + 1));
                    }

                    int dropOrNot = new Random().Next(0, 4);

                    ItemsContainer inventory = character.GetInventoryContainer();
                    if (inventory != null && dropOrNot == 0)
                    {
                        int freeSlot = inventory.GetFreeSlot();
                        if (freeSlot != -1)
                        {
                            inventory.AddItem(freeSlot, new ItemData()
                            {
                                baseId = 1,
                                uniqueId = 1
                            });
                        }
                    }
                }
            }
        }
    }
}
