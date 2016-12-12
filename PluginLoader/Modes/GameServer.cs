namespace PluginLoader
{
    class GameServer : GameMode
    {
        private readonly Game.GameStates.GameServer gameServer;
        public GameServer(PluginManager pluginManager, Game.GameStates.GameServer gameServer) : base(pluginManager)
        {
            this.gameServer = gameServer;
        }
        public override void StartGameMode()
        {
            base.StartGameMode();

            pluginManager.Start();

            gameServer.Controllers.Players.OnAddPlayer += Players_OnAddPlayer;

            //foreach (Package package in pluginManager.PackageManager.Packages)
            //{
            //    Console.WriteLine($"[{nameof(PluginManager)}] Pushing {filename} into filesystem");
            //    AddFileToFS(gameServer.Controllers.Network.NetFS, nameof(PluginLoader), filename, File.ReadAllBytes(package.ArchivePath));
            //}

            //var welcomePacket = new ServerPLWelcome
            //{
            //};
            //this.gameServer.Controllers.Players.OnAddPlayer += delegate (Game.Server.Player player)
            //{
            //    player.SendRPC(welcomePacket);
            //};
        }

        private void Players_OnAddPlayer(Game.Server.Player obj)
        {
            gameServer.Controllers.Chat.SendToPlayer(obj, Game.Configuration.Config.Singleton.NotificationChatColor, $"This server is using {nameof(PluginLoader)} v{Versions.PLVersion}", nameof(PluginLoader), "Server");
        }
    }
}
