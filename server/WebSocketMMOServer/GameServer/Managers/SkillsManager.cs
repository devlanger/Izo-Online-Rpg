using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using WebSocketMMOServer.Database;
using WebSocketMMOServer.GameServer.Packets;

namespace WebSocketMMOServer.GameServer
{
    public class SkillsManager
    {
        public Dictionary<int, SkillData> skills = new Dictionary<int, SkillData>();


        public SkillsManager()
        {
            DataTable mobsProtoTable = DatabaseManager.ReturnQuery("SELECT * FROM skills_proto");
            for (int i = 0; i < mobsProtoTable.Rows.Count; i++)
            {
                DataRow row = mobsProtoTable.Rows[i];

                SkillData data = new SkillData()
                {
                    baseId = (int)row["id"],
                    name = (string)row["name"],
                    reqLevel = (byte)row["req_level"],
                    cooldown = (double)row["cooldown"],
                    minDmg = (int)row["min_dmg"],
                    maxDmg = (int)row["max_dmg"],
                };

                skills.Add(data.baseId, data);
            }
        }

        public SkillData GetSkill(int targetId)
        {
            if(skills.ContainsKey(targetId))
            {
                return skills[targetId];
            }

            return null;
        }
    }
    public class SkillData
    {
        public int baseId;
        public string name;
        public int reqLevel;
        public double cooldown;
        public int minDmg;
        public int maxDmg;
    }
}
