using System;
using System.Collections.Generic;
using System.Text;
using WebSocketMMOServer.Database;

namespace WebSocketMMOServer.GameServer
{
    public class ServerManager
    {
        public static ServerManager Instance { get; private set; }

        public DatabaseManager DatabaseManager { get; private set; }
        public ItemsManager ItemsManager { get; private set; }
        public StatsManager StatsManager { get; private set; }
        public ClientsManager ClientsManager { get; private set; }
        public CharactersManager CharactersManager { get; private set; }
        public TickManager TickManager { get; private set; }
        public CombatManager CombatManager { get; private set; }
        public RespawnManager RespawnManager { get; private set; }
        public FieldOfViewManager fovManager { get; private set; }
        public SkillsManager SkillsManager { get; private set; }

        public ServerManager()
        {
            Instance = this;

            TickManager = new TickManager();
            DatabaseManager = new DatabaseManager();
            ItemsManager = new ItemsManager();
            StatsManager = new StatsManager();
            SkillsManager = new SkillsManager();
            CharactersManager = new CharactersManager();
            ClientsManager = new ClientsManager();
            CombatManager = new CombatManager(TickManager, CharactersManager);
            RespawnManager = new RespawnManager(TickManager, CharactersManager);
            fovManager = new FieldOfViewManager(TickManager, CharactersManager);
        }
    }
}
