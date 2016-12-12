namespace PluginLoader
{
    class GameClient : GameMode
    {
        private readonly Game.GameStates.GameClient gameClient;

        public GameClient(PluginManager pluginManager, Game.GameStates.GameClient gameClient) : base(pluginManager)
        {
            this.gameClient = gameClient;
        }
        public override void StartGameMode()
        {
            base.StartGameMode();

            pluginManager.Start();

            //gameClient.Controllers.Network.Net.RpcDispatcher.Functions[typeof(ServerPluginLoaderWelcome)] =
            //    new RPCDelegate(delegate (RPCData data)
            //    {
            //        var deserializedObject = (ServerPluginLoaderWelcome)data.DeserializedObject;
            //        this.pluginManager.Log.Info($"Server is using PluginLoader v{deserializedObject.Version.ToString()}", "PluginLoader");
            //        deserializedObject.PluginList.ForEach(delegate(string dll)
            //        {
            //            this.pluginManager.Log.Info($"Requesting plugin {dll}", "PluginLoader");
            //            var loadingScreen = Globals.loadingScreen;
            //            Globals.loadingScreen.RenderLoadingScreenOnce(0, "Loading plugins");
            //            this.gameClient.Controllers.Network.NetFS.Download(
            //                this.gameClient.Controllers.Network.Net.ServerConnection(), $"pluginloader_{dll.ToLower()}", "pluginloader", dll, true, delegate (NetFilesystem.TransferStatus status, NetFilesystem.TransferState state)
            //                {
            //                });
            //        });
            //    });
        }
    }
}