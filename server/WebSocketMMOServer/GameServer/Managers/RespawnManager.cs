using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace WebSocketMMOServer.GameServer
{
    public class RespawnManager
    {
        private CharactersManager charactersManager;
        private TickManager tickManager;

        public RespawnManager(TickManager tickManager, CharactersManager charactersManager)
        {
            this.tickManager = tickManager;
            this.charactersManager = charactersManager;

            tickManager.OnTick += TickManager_OnTick;
        }

        private void TickManager_OnTick()
        {
            /*foreach (var character in new Dictionary<int, Character>(charactersManager.characters))
            {
                if(character.Value.IsDead)
                {

                }
            }*/
        }
    }
}
