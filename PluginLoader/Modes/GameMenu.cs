namespace PluginLoader
{
    class GameMenu : GameMode
    {
        private readonly Game.GameStates.GameMainMenu gameMenu;

        public GameMenu(PluginManager pluginManager, Game.GameStates.GameMainMenu gameMenu) : base(pluginManager)
        {
            this.gameMenu = gameMenu;
        }
        public override void StartGameMode()
        {
            base.StartGameMode();

            pluginManager.Start();
        }
    }
}