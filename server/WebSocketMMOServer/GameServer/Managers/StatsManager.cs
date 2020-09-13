using System;
using System.Collections.Generic;
using System.Text;
using WebSocketMMOServer.Database;

namespace WebSocketMMOServer.GameServer
{
    public class StatsManager
    {
        public Dictionary<int, StatsContainer> charactersStats = new Dictionary<int, StatsContainer>();

        public void AddStatsForCharacter(Character character)
        {
            if(charactersStats.ContainsKey(character.Id))
            {
                Console.WriteLine("Trying to duplicate stats container for id: " + character.Id);
                return;
            }

            StatsContainer container = new StatsContainer(this);
            charactersStats.Add(character.Id, container);
        }

        public void RemoveStatsForCharacter(int characterId)
        {
            if(charactersStats.ContainsKey(characterId))
            {
                charactersStats.Remove(characterId);
            }
        }

        public void SaveStatisticsToDatabase(Player selectedCharacter)
        {
            string query = string.Format(@"INSERT INTO characters(id, lvl, pos_x, pos_z, exp) VALUES('{0}', '{1}', '{2}', '{3}', '{4}') ON DUPLICATE KEY UPDATE 
                                lvl = VALUES(lvl), pos_x = VALUES(pos_x), pos_z = VALUES(pos_z), exp = VALUES(exp)", 
                                selectedCharacter.DatabaseId,
                                (short)GetStat(selectedCharacter.Id, StatType.LEVEL), 
                                (short)GetStat(selectedCharacter.Id, StatType.POS_X),
                                (short)GetStat(selectedCharacter.Id, StatType.POS_Z),
                                (int)GetStat(selectedCharacter.Id, StatType.EXPERIENCE));

            DatabaseManager.InsertQuery(query);
        }

        public StatsContainer GetContainerForCharacter(int characterId)
        {
            if (charactersStats.ContainsKey(characterId))
            {
                return charactersStats[characterId];
            }
            else
            {
                return null;
            }
        }

        public bool GetContainerForCharacter(int characterId, out StatsContainer container)
        {
            if (charactersStats.ContainsKey(characterId))
            {
                container = charactersStats[characterId];
                return true;
            }
            else
            {
                container = null;
                return false;
            }
        }

        public object GetStat(int characterId, StatType stat)
        {
            if (charactersStats.ContainsKey(characterId))
            {
                return charactersStats[characterId].GetStat(stat).value;
            }
            else
            {
                return null;
            }
        }

        public bool GetStat(int characterId, StatType stat, out StatObject s)
        {
            if(charactersStats.ContainsKey(characterId))
            {
                s = charactersStats[characterId].GetStat(stat);
                return true;
            }
            else
            {
                s = default(StatObject);
                return false;
            }
        }
    }

    public class StatObject
    {
        public StatType statType;
        public object value;
    }

    public class StatsContainer
    {
        private StatsManager manager;
        private Dictionary<StatType, object> stats = new Dictionary<StatType, object>()
        {
            { StatType.NAME, "Object" },
            { StatType.LEVEL, (short)1 },
            { StatType.HEALTH, (int)100 },
            { StatType.MAX_HEALTH, (int)100 },
            { StatType.MANA, (int)100 },
            { StatType.MAX_MANA, (int)100 },
            { StatType.EXPERIENCE, (int)0 },
            { StatType.GOLD, (int)0 },
            { StatType.RACE, (byte)0 },
            { StatType.CLASS, (byte)0 },
            { StatType.POS_X, (short)0 },
            { StatType.POS_Z, (short)0 },
            { StatType.TARGET_ID, (int)-1 },
        };

        public StatsContainer(StatsManager manager)
        {
            this.manager = manager;
        }

        public StatObject GetStat(StatType stat)
        {
            if (stats.ContainsKey(stat))
            {
                return new StatObject()
                {
                    statType = stat,
                    value = stats[stat]
                };
            }

            Console.WriteLine("ERROR: Cannot retrieve stat value for stat:" + stat);
            return null;
        }

        public void SetStat(StatType type, object v)
        {
            stats[type] = v;
        }
    }
}
