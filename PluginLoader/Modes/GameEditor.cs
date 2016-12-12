namespace PluginLoader
{
    class GameEditor : GameMode
    {
        private readonly Game.GameStates.GameShipEditor gameEditor;

        public GameEditor(PluginManager pluginManager, Game.GameStates.GameShipEditor gameEditor) : base(pluginManager)
        {
            this.gameEditor = gameEditor;
        }
        public override void StartGameMode()
        {
            base.StartGameMode();

            pluginManager.Start();
        }
    }
}