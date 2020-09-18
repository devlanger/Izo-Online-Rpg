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
    public class CharactersManager
    {
        public Dictionary<int, Character> characters = new Dictionary<int, Character>();
        public Dictionary<int, Player> players = new Dictionary<int, Player>();
        public Dictionary<int, Client> clients = new Dictionary<int, Client>();

        public static int lastId = 1;

        public CharactersManager()
        {
            DataTable mobsTable = DatabaseManager.ReturnQuery("SELECT * FROM mobs");
            for (int i = 0; i < mobsTable.Rows.Count; i++)
            {
                DataRow row = mobsTable.Rows[i];

                int id = lastId++;
                Character x = CharactersManager.CreateCharacter(id);
                StatsContainer stats = x.GetStatsContainer();
                stats.SetStat(StatType.NAME, "Gnome");
                stats.SetStat(StatType.POS_X, (short)row["pos_x"]);
                stats.SetStat(StatType.POS_Z, (short)row["pos_z"]);

                AddCharacter(x);
            }
        }

        public Character GetCharacterById(int targetId)
        {
            if(characters.ContainsKey(targetId))
            {
                return characters[targetId];
            }

            return null;
        }

        public static Character CreateCharacter(int id)
        {
            Character character = new Character(id);
            ServerManager.Instance.StatsManager.AddStatsForCharacter(character);
            return character;
        }

        public static Player CreatePlayer()
        {
            Player character = new Player(lastId++);
            ServerManager.Instance.StatsManager.AddStatsForCharacter(character);
            ServerManager.Instance.ItemsManager.AddInventoryForCharacter(character);
            return character;
        }

        public void DespawnCharacter(Character character)
        {
            foreach (var item in ServerManager.Instance.ClientsManager.clients)
            {
                if (item.Value.LoggedIn && item.Value.InGameWorld)
                {
                    Server.Instance.SendData(item.Value.ip, new DespawnCharacterPacket(character));
                }
            }

            if (character is Player)
            {
                ServerManager.Instance.StatsManager.SaveStatisticsToDatabase((Player)character);
                ServerManager.Instance.ItemsManager.RemoveInventoryForCharacter(character.Id);
            }

            ServerManager.Instance.StatsManager.RemoveStatsForCharacter(character.Id);
            RemoveCharacter(character);
        }

        public Dictionary<int, Client> GetClientsInRange(Vector3 pos, float range = 50, int ignoreId = -1)
        {
            Dictionary<int, Client> result = new Dictionary<int, Client>();
            foreach (var item in clients.ToArray())
            {
                if(!item.Value.InGameWorld)
                {
                    continue;
                }

                if(item.Value.SelectedCharacter.Id == ignoreId)
                {
                    continue;
                }

                if (Vector3.Distance(pos, item.Value.SelectedCharacter.Position) < range)
                {
                    result.Add(item.Key, item.Value);
                }
            }

            return result;
        }

        public Dictionary<int, T> GetCharactersInRange<T>(Vector3 pos, float range = 50, int ignoreId = -1) where T : Character
        {
            Dictionary<int, T> result = new Dictionary<int, T>();
            foreach (var item in characters.Values.ToArray())
            {
                if(ignoreId == item.Id)
                {
                    continue;
                }

                float distance = Vector3.Distance(pos, item.Position);
                if(distance < range)
                {
                    result.Add(item.Id, item as T);
                }
            }

            return result;
        }

        public void AddCharacter(Character character)
        {
            if (character is Player)
            {
                players.Add(character.Id, (Player)character);
            }

            characters.Add(character.Id, character);
        }

        public void RemoveCharacter(Character character)
        {
            if(character is Player)
            {
                players.Remove(character.Id);
            }

            if(clients.ContainsKey(character.Id))
            {
                clients.Remove(character.Id);
            }

            characters.Remove(character.Id);
        }
    }
}
