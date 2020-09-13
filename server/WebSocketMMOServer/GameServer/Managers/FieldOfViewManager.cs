using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using WebSocketMMOServer.GameServer.Packets;
using WebSocketMMOServer.GameServer.Packets.Outgoing;

namespace WebSocketMMOServer.GameServer
{
    public class FieldOfViewManager
    {
        private CharactersManager charactersManager;
        private TickManager tickManager;

        private float lastCheckTime = 0;

        public FieldOfViewManager(TickManager tickManager, CharactersManager charactersManager)
        {
            lastCheckTime = tickManager.Time;

            this.tickManager = tickManager;
            this.charactersManager = charactersManager;

            tickManager.OnTick += TickManager_OnTick;
        }

        private void TickManager_OnTick()
        {
            if (tickManager.Time > lastCheckTime + 1)
            {
                Dictionary<int, Client> clients = new Dictionary<int, Client>(charactersManager.clients);
                foreach (var client in clients)
                {
                    Character character = client.Value.SelectedCharacter;
                    Dictionary<int, Character> inRange = charactersManager.GetCharactersInRange<Character>(character.Position, 50, character.Id);
                    HashSet<int> seen = character.lastSeenCharacters;
                    HashSet<int> newList = new HashSet<int>();

                        foreach (var item in inRange)
                        {
                            if(seen.Contains(item.Value.Id))
                            {
                                seen.Remove(item.Value.Id);
                            }
                            else
                            {
                                Server.Instance.SendData(client.Value.ip, new SpawnCharacterPacket(item.Value));
                            }

                            newList.Add(item.Value.Id);
                        }

                        foreach (var id in seen)
                        {
                            Server.Instance.SendData(client.Value.ip, new DespawnCharacterPacket(id));
                        }

                        character.lastSeenCharacters = newList;
                    }
                }

            lastCheckTime = tickManager.Time;
        }
    }
}
