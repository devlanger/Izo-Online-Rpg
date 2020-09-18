using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using WebSocketMMOServer.Database;
using WebSocketMMOServer.GameServer;
using WebSocketMMOServer.GameServer.Packets.Outgoing;

namespace WebSocketMMOServer
{
    public class GamePacketsImpl
    {
        private static Dictionary<GamePacketType, Action<Client, BinaryReader>> packetsImplementation = new Dictionary<GamePacketType, Action<Client, BinaryReader>>()
        {
            { GamePacketType.LOGIN_REQUEST, LoginRequestImpl },
            { GamePacketType.SET_DESTINATION, SetDestinationImpl },
            { GamePacketType.CREATE_CHARACTER, CreateCharacterImpl },
            { GamePacketType.SET_ATTACK_TARGET, SetAttackTargetImpl },
            { GamePacketType.USE_SKILL, UseSkillImpl },
        };

        private static void UseSkillImpl(Client arg1, BinaryReader reader)
        {
            int skillId = reader.ReadInt32();

            Console.WriteLine("Use skill: " + skillId);
            int targetId = (int)arg1.SelectedCharacter.GetStatsContainer().GetStat(StatType.TARGET_ID).value;
            Character target = ServerManager.Instance.CharactersManager.GetCharacterById(targetId);
            if(target != null)
            {
                if(target.IsDead)
                {
                    return;
                }

                foreach (var client in ServerManager.Instance.CharactersManager.GetClientsInRange(arg1.SelectedCharacter.Position))
                {
                    Server.Instance.SendData(client.Value.ip, new ExecuteUseSkillPacket(arg1.SelectedCharacter.Id, targetId, skillId));
                }

                ServerManager.Instance.CombatManager.DealDamage(arg1.SelectedCharacter, target, new AttackData()
                {
                    attackerId = arg1.SelectedCharacter.Id,
                    targetId = target.Id,
                    damage = (ushort)new Random().Next(30, 60),
                    damageType = 0
                });
            }
        }

        private static void SetAttackTargetImpl(Client arg1, BinaryReader reader)
        {
            int targetId = reader.ReadInt32();
            byte action = reader.ReadByte();

            switch(action)
            {
                //SELECT
                case (byte)0:
                    arg1.SelectedCharacter.GetStatsContainer().SetStat(StatType.TARGET_ID, (int)targetId);
                    arg1.SelectedCharacter.SelectionState = SelectionState.SELECTION;
                    break;

                //ATTACK
                case (byte)1:
                    arg1.SelectedCharacter.GetStatsContainer().SetStat(StatType.TARGET_ID, (int)targetId);
                    arg1.SelectedCharacter.SelectionState = SelectionState.ATTACK;
                    
                    break;
            }
        }

        public static void ExecutePacket(GamePacketType type, Client client, BinaryReader reader)
        {
            if(packetsImplementation.ContainsKey(type))
            {
                packetsImplementation[type].Invoke(client, reader);
            }
        }

        private static void LoginRequestImpl(Client client, BinaryReader reader)
        {
            string username = reader.ReadString();
            string password = reader.ReadString();

            DataTable accountsTable = DatabaseManager.ReturnQuery(string.Format("SELECT * FROM accounts WHERE username='{0}' AND password='{1}'", username, password));
            if(accountsTable.Rows.Count > 0)
            {
                DataRow accountData = accountsTable.Rows[0];
                int accountId = (int)accountData["id"];
                client.accountId = accountId;
                EnterCharacterToGameWorld(client, accountId);
            }
        }

        private static void EnterCharacterToGameWorld(Client client, int accountId)
        {
            DataTable characterTable = DatabaseManager.ReturnQuery(string.Format("SELECT * FROM characters WHERE account_id='{0}'", accountId));
            if (characterTable.Rows.Count > 0)
            {
                DataRow characterData = characterTable.Rows[0];
                int characterId = (int)characterData["id"];
                Player character = CharactersManager.CreatePlayer();
                character.DatabaseId = characterId;

                client.SelectedCharacter = character;

                if (ServerManager.Instance.StatsManager.GetContainerForCharacter(client.SelectedCharacter.Id, out StatsContainer container))
                {
                    container.SetStat(StatType.NAME, (string)characterData["name"]);
                    container.SetStat(StatType.LEVEL, (short)characterData["lvl"]);
                    container.SetStat(StatType.RACE, (byte)characterData["ch_class"]);
                    container.SetStat(StatType.CLASS, (byte)characterData["race"]);
                    container.SetStat(StatType.POS_X, (short)characterData["pos_x"]);
                    container.SetStat(StatType.POS_Z, (short)characterData["pos_z"]);
                    container.SetStat(StatType.EXPERIENCE, (int)characterData["exp"]);
                }

                client.BindEvents();

                ServerManager.Instance.CharactersManager.AddCharacter(character);
                ServerManager.Instance.CharactersManager.clients.Add(character.Id, client);

                Dictionary<int, Character> table = ServerManager.Instance.CharactersManager.GetCharactersInRange<Character>(client.SelectedCharacter.Position, 50);
                //Send all characters
                foreach (var item in table)
                {
                    Server.Instance.SendData(client.ip, new SpawnCharacterPacket(item.Value));
                }

                //Acknowledge other clients
                foreach (var item in ServerManager.Instance.CharactersManager.GetClientsInRange(client.SelectedCharacter.Position, 50))
                {
                    Server.Instance.SendData(item.Value.ip, new SpawnCharacterPacket(client.SelectedCharacter));
                }

                Packet packet = new Packet();
                packet.writer.Write((byte)1);
                packet.writer.Write((int)client.SelectedCharacter.Id);
                packet.writer.Write((short)characterData["pos_x"]);
                packet.writer.Write((short)characterData["pos_z"]);
                packet.writer.Write((short)characterData["lvl"]);
                packet.writer.Write((int)characterData["exp"]);

                Server.Instance.SendData(client.ip, packet);
            }
            else
            {
                Server.Instance.SendData(client.ip, new CharacterSelectionPacket());
            }
        }

        private static void CreateCharacterImpl(Client client, BinaryReader reader)
        {
            string nickname = reader.ReadString();
            byte @class = reader.ReadByte();
            byte race = reader.ReadByte();
            short posX = 70;
            short posZ = 105;

            if(!client.LoggedIn)
            {
                return;
            }

            string query = string.Format(@"INSERT INTO characters(account_id, name, lvl, race, ch_class, pos_x, pos_z) VALUES('{0}','{1}','{2}', '{3}', '{4}', '{5}', '{6}')",
                client.accountId, nickname, 1, race, @class, posX, posZ);

            long id = DatabaseManager.InsertQuery(query);
            if(id != -1)
            {
                EnterCharacterToGameWorld(client, client.accountId);
            }
            else
            {
                Console.WriteLine("Error creating character: " + nickname);
            }
        }

        private static void SetDestinationImpl(Client client, BinaryReader reader)
        {
            short posX = reader.ReadInt16();
            short posZ = reader.ReadInt16();
            SetDestinationLogic(client, posX, posZ);
        }

        private static void SetDestinationLogic(Client client, short posX, short posZ)
        {
            client.SelectedCharacter.SetDestination(posX, posZ);
        }
    }
}
