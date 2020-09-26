using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            { GamePacketType.REQUEST_ITEM_ACTION, RequestItemActionImpl },
            { GamePacketType.CHAT_MESSAGE_PACKET, ChatMessageImpl},
            { GamePacketType.REGISTER_REQUEST, RegisterRequestImpl },
        };

        private static void RegisterRequestImpl(Client client, BinaryReader reader)
        {
            string username = reader.ReadString();
            string password = reader.ReadString();
            string password2 = reader.ReadString();
            string email = reader.ReadString();

            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(password2) || string.IsNullOrEmpty(email))
            {
                return;
            }

            if(password != password2)
            {
                return;
            }

            long registerAccountId = DatabaseManager.InsertQuery(string.Format("INSERT INTO accounts(username, password, email) VALUES ('{0}', '{1}', '{2}')", username, password, email));

            if(registerAccountId == -1)
            {
                return;
            }

            LoginExecute(client, username, password);
        }

        private static void ChatMessageImpl(Client arg1, BinaryReader reader)
        {
            string msg = reader.ReadString();
            msg = msg.Insert(0, string.Format("{0}: ", (string)arg1.SelectedCharacter.GetStat(StatType.NAME)));
            foreach (var item in ServerManager.Instance.CharactersManager.clients)
            {
                Server.Instance.SendData(item.Value.ip, new ChatMessagePacket(msg));
            }
        }

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

                var data = ServerManager.Instance.SkillsManager.GetSkill(skillId);
                if (data != null)
                {
                    StatsContainer stats = arg1.SelectedCharacter.GetStatsContainer();
                    if (stats.CanUseSkill(data))
                    {
                        float time = ServerManager.Instance.TickManager.Time;
                        if (!stats.skillsUseTime.TryAdd(data.baseId, time))
                        {
                            stats.skillsUseTime[data.baseId] = time;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
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
                    damage = (ushort)new Random().Next(data.minDmg, data.maxDmg),
                    damageType = 0
                });
            }
        }

        public enum ItemAction
        {
            MOVE = 1,
            DELETE = 2,
            USE = 3,
        }

        private static void RequestItemActionImpl(Client arg1, BinaryReader reader)
        {
            ItemAction action = (ItemAction)reader.ReadByte();

            ItemsContainerId sourceContainerId = (ItemsContainerId)reader.ReadByte();
            ItemsContainerId targetContainerId = (ItemsContainerId)reader.ReadByte();
            
            int sourceSlot = reader.ReadInt32();
            int targetSlot = reader.ReadInt32();

            var containers = ServerManager.Instance.ItemsManager.GetContainers(arg1.SelectedCharacter.Id);

            switch (action)
            {
                case ItemAction.MOVE:
                    var sourceContainer = containers[sourceContainerId];
                    var targetContainer = containers[targetContainerId];

                    if (targetContainerId == ItemsContainerId.SHOP)
                    {
                        if (sourceContainerId == ItemsContainerId.INVENTORY)
                        {
                            SellItem(arg1, sourceSlot, sourceContainer);
                        }


                        return;
                    }

                    if (sourceContainerId == ItemsContainerId.SHOP)
                    {
                        if (targetContainerId == ItemsContainerId.INVENTORY)
                        {
                            BuyItem(arg1, sourceSlot);
                        }
                        return;
                    }

                    if (sourceContainer.Items.ContainsKey(sourceSlot))
                    {
                        var sourceItem = sourceContainer.Items[sourceSlot];

                        if (targetContainerId == ItemsContainerId.EQUIPMENT)
                        {
                            //EQUIP ITEM
                            if (ServerManager.Instance.ItemsManager.GetItemPrototype(sourceItem.baseId, out var prototype))
                            {
                                if((short)arg1.SelectedCharacter.GetStat(StatType.LEVEL) < prototype.reqLvl)
                                {
                                    return;
                                }
                            }
                        }

                        ItemData targetItem = null;

                        if (targetContainer.Items.ContainsKey(targetSlot))
                        {
                            targetItem = targetContainer.Items[targetSlot];
                        }

                        sourceContainer.RemoveItem(sourceSlot, true); 
                        
                        if (targetItem != null)
                        {
                            targetContainer.RemoveItem(targetSlot, true);
                        }

                        targetContainer.AddItem(targetSlot, sourceItem, true);

                        if (targetItem != null)
                        {
                            sourceContainer.AddItem(sourceSlot, targetItem, true);
                        }

                        containers[sourceContainerId].Refresh();
                        containers[targetContainerId].Refresh();
                    }
                    break;
            }
        }

        private static void SellItem(Client arg1, int sourceSlot, ItemsContainer sourceContainer)
        {
            Console.WriteLine("Sell item");
            sourceContainer.RemoveItem(sourceSlot);
            arg1.SelectedCharacter.AddStatInt(StatType.GOLD, 100);
        }

        private static void BuyItem(Client arg1, int sourceSlot)
        {
            Console.WriteLine("Buy item slot: " + sourceSlot);

            ShopContainer shop = ServerManager.Instance.ShopManager.GetShop(((Player)arg1.SelectedCharacter).SelectedVendorId);
            if (shop != null)
            {
                if (shop.items.GetItem(sourceSlot, out ItemData data))
                {
                    if (ServerManager.Instance.ItemsManager.GetItemPrototype(data.baseId, out ItemPrototype prototype))
                    {
                        if ((int)arg1.SelectedCharacter.GetStat(StatType.GOLD) >= prototype.price)
                        {
                            int freeInventorySlot = arg1.SelectedCharacter.GetItemsContainer(ItemsContainerId.INVENTORY).GetFreeSlot();
                            if (freeInventorySlot != -1)
                            {
                                ItemData boughtItem = ServerManager.Instance.ItemsManager.CreateItemData(new ItemData()
                                {
                                    baseId = data.baseId,
                                });

                                arg1.SelectedCharacter.AddStatInt(StatType.GOLD, -prototype.price);
                                arg1.SelectedCharacter.GetItemsContainer(ItemsContainerId.INVENTORY).AddItem(freeInventorySlot, boughtItem);
                            }
                        }
                    }
                }
            }
        }

        private static void SetAttackTargetImpl(Client client, BinaryReader reader)
        {
            int targetId = reader.ReadInt32();
            byte action = reader.ReadByte();

            Character c = ServerManager.Instance.CharactersManager.GetCharacterById(targetId);
            if (c == null)
            {
                return;
            }

            switch (action)
            {
                //SELECT
                case (byte)0:
                    client.SelectedCharacter.GetStatsContainer().SetStat(StatType.TARGET_ID, (int)targetId);
                    client.SelectedCharacter.SelectionState = SelectionState.SELECTION;
                    
                    break;

                //ATTACK
                case (byte)1:
                    if (c.OnClick ==  ClickType.MOB)
                    {
                        client.SelectedCharacter.GetStatsContainer().SetStat(StatType.TARGET_ID, (int)targetId);
                        client.SelectedCharacter.SelectionState = SelectionState.ATTACK;
                    }

                    if (c.OnClick == ClickType.SHOP)
                    {
                        if (ServerManager.Instance.ShopManager.GetShop(c.BaseId, out ShopContainer shopContainer))
                        {
                            ((Player)client.SelectedCharacter).SelectedVendorId = c.BaseId;
                            Server.Instance.SendData(client.ip, new SyncInventoryPacket(shopContainer.items));
                        }
                    }

                    break;
            }
        }

        public static void ExecutePacket(GamePacketType type, Client client, BinaryReader reader)
        {
            try
            {
                if (packetsImplementation.ContainsKey(type))
                {
                    packetsImplementation[type].Invoke(client, reader);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void LoginRequestImpl(Client client, BinaryReader reader)
        {
            string username = reader.ReadString();
            string password = reader.ReadString();
            
            LoginExecute(client, username, password);
        }

        private static void LoginExecute(Client client, string username, string password)
        {
            DataTable accountsTable = DatabaseManager.ReturnQuery(string.Format("SELECT * FROM accounts WHERE username='{0}' AND password='{1}'", username, password));
            if (accountsTable.Rows.Count > 0)
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
                    container.SetStat(StatType.RACE, (byte)characterData["race"]);
                    container.SetStat(StatType.CLASS, (byte)characterData["ch_class"]);
                    container.SetStat(StatType.POS_X, (short)characterData["pos_x"]);
                    container.SetStat(StatType.POS_Z, (short)characterData["pos_z"]);
                    container.SetStat(StatType.EXPERIENCE, (int)characterData["exp"]);
                    container.SetStat(StatType.KINGDOM, (byte)characterData["kingdom"]);
                    container.SetStat(StatType.GOLD, (int)characterData["gold"]);
                }

                Dictionary<ItemsContainerId, ItemsContainer> containers = ServerManager.Instance.ItemsManager.GetContainers(character.Id);
                DataTable itemsTable = DatabaseManager.ReturnQuery(string.Format("SELECT * FROM items WHERE owner_id='{0}'", character.DatabaseId));
                for (int i = 0; i < itemsTable.Rows.Count; i++)
                {
                    DataRow itemRow = itemsTable.Rows[i];
                    ItemsContainerId inventoryId = (ItemsContainerId)(byte)itemRow["inventory_id"];

                    containers[inventoryId].AddItem((int)itemRow["slot"], new ItemData()
                    {
                        uniqueId = (int)itemRow["id"],
                        baseId = (int)itemRow["base_id"],
                        amount = (int)itemRow["amount"],
                    });
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
                packet.writer.Write((int)characterData["gold"]);

                Server.Instance.SendData(client.ip, packet);
                foreach (var ic in containers)
                {
                    Server.Instance.SendData(client.ip, new SyncInventoryPacket(ic.Value));
                }
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
            byte kingdom = reader.ReadByte();

            short posX = 70;
            short posZ = 105;

            posX = (short)KingdomsManager.kingdoms[kingdom].spawnPoint.X;
            posZ = (short)KingdomsManager.kingdoms[kingdom].spawnPoint.Z;
            
            if(!client.LoggedIn)
            {
                return;
            }

            string query = string.Format(@"INSERT INTO characters(account_id, name, lvl, race, ch_class, pos_x, pos_z, kingdom) VALUES('{0}','{1}','{2}', '{3}', '{4}', '{5}', '{6}', '{7}')",
                client.accountId, nickname, 1, race, @class, posX, posZ, kingdom);

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

            client.SelectedCharacter.SelectionState = SelectionState.SELECTION;
            SetDestinationLogic(client, posX, posZ);
        }

        private static void SetDestinationLogic(Client client, short posX, short posZ)
        {
            client.SelectedCharacter.SetDestination(posX, posZ);
        }
    }
}
