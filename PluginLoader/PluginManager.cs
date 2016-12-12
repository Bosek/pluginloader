using Game.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace PluginLoader
{
    public delegate void PLEvent(object arg = null);
    public class PluginManager
    {
        private readonly PackageManager PackageManager = new PackageManager();
        private readonly PluginInjector pluginInjector;

        private Game.GameStates.GameClient gameClient;
        private Game.GameStates.GameShipEditor gameEditor;
        private Game.GameStates.GameMainMenu gameMenu;
        private Game.GameStates.GameServer gameServer;

        private bool initialized;

        internal PluginManager(PluginInjector pluginInjector)
        {
            this.pluginInjector = pluginInjector;
            setupLog();

            pluginInjector.OnAgosGuiInitialized += delegate (object arg) { OnAgosGuiInitialized?.Invoke(arg); };
            pluginInjector.OnGameClientActivated += delegate (object arg) { OnGameClientActivated?.Invoke(arg); };
            pluginInjector.OnGameClientDeactivated += delegate (object arg) { OnGameClientDeactivated?.Invoke(arg); };
            pluginInjector.OnGameClientInitialized += delegate (object arg)
            {
                initialize();

                gameClient = (Game.GameStates.GameClient)arg;
                new GameClient(this, gameClient).StartGameMode();

                OnGameClientInitialized?.Invoke(gameClient);
            };
            pluginInjector.OnGameClientUnload += delegate (object arg) { OnGameClientUnload?.Invoke(arg); };
            pluginInjector.OnGameClientUpdate += delegate (object arg) { OnGameClientUpdate?.Invoke(arg); };
            pluginInjector.OnGameEditorInitialized += delegate (object arg)
            {
                initialize();

                gameEditor = (Game.GameStates.GameShipEditor)arg;
                new GameEditor(this, gameEditor).StartGameMode();

                OnGameEditorInitialized?.Invoke(arg);
            };
            pluginInjector.OnGameEditorUnload += delegate (object arg) { OnGameEditorUnload?.Invoke(arg); };
            pluginInjector.OnGameEditorUpdate += delegate (object arg) { OnGameEditorUpdate?.Invoke(arg); };
            pluginInjector.OnGameMenuInitialized += delegate (object arg)
            {
                initialize();

                gameMenu = (Game.GameStates.GameMainMenu)arg;
                new GameMenu(this, gameMenu).StartGameMode();

                OnGameMenuInitialized?.Invoke(arg);
            };
            pluginInjector.OnGameServerInitialized += delegate (object arg)
            {
                initialize();

                gameServer = (Game.GameStates.GameServer)arg;
                new GameServer(this, gameServer).StartGameMode();

                OnGameServerInitialized?.Invoke(arg);
            };
            pluginInjector.OnGameServerUnload += delegate (object arg) { OnGameServerUnload?.Invoke(arg); };
            pluginInjector.OnGameServerUpdate += delegate (object arg) { OnGameServerUpdate?.Invoke(arg); };
        }

        public event PLEvent OnAgosGuiInitialized;
        public event PLEvent OnClientHotReload;
        public event PLEvent OnEditorHotReload;
        public event PLEvent OnGameClientActivated;
        public event PLEvent OnGameClientDeactivated;
        public event PLEvent OnGameClientInitialized;
        public event PLEvent OnGameClientUnload;
        public event PLEvent OnGameClientUpdate;
        public event PLEvent OnGameEditorInitialized;
        public event PLEvent OnGameEditorUnload;
        public event PLEvent OnGameEditorUpdate;
        public event PLEvent OnGameMenuInitialized;
        public event PLEvent OnGameServerInitialized;
        public event PLEvent OnGameServerUnload;
        public event PLEvent OnGameServerUpdate;
        public event PLEvent OnMenuHotReload;
        public event PLEvent OnServerHotReload;
        public void Start()
        {
            try
            {
                var alreadyStarted = PackageManager.Packages != null && PackageManager.Packages.Any();
                if (!alreadyStarted)
                    Log.Info("Starting");
                else
                    Log.Info("Restarting");

                loadPackages();
                var packages = PackageManager.Packages.ToArray();

                Log.Info("Executing packages");
                var executionError = false;

                PackageManager.Packages
                    .ForEach(package =>
                    {
                        var scriptRunner = new PYScriptRunner();
                        var scriptGlobals = new ScriptGlobal
                        {
                            GameDirectory = PackageManager.GameDirectory,
                            PackageDirectory = package.Path,
                            Packages = packages,
                            PluginManager = this,
                            Versions = typeof(Versions)
                        };
                        try
                        {
                            scriptRunner.CreateScope(scriptGlobals);
                            scriptRunner.ExecuteScript(System.IO.Path.Combine(package.Path, package.Metadata.EntryPoint));

                            if (scriptRunner.Warnings.Any())
                                scriptRunner.Warnings.ToList().ForEach(warning => Log.Warning(warning));
                        }
                        catch (Exception exception)
                        {
                            executionError = true;
                            Log.Error($"Error while executing {package.Metadata.ToString()}");
                            if (scriptRunner.Errors.Any())
                                scriptRunner.Errors.ToList().ForEach(error => Log.Error(error));
                            else
                                Log.Exception(exception);
                        }
                    });
                if (executionError)
                {
                    Log.Error("Please resolve issues and restart the game");
                    Console.ReadLine();
                }
                else
                    Log.Info("Packages executed");

                if (alreadyStarted && gameClient != null)
                    OnClientHotReload?.Invoke(gameClient);
                if (alreadyStarted && gameEditor != null)
                    OnEditorHotReload?.Invoke(gameEditor);
                if (alreadyStarted && gameMenu != null)
                    OnMenuHotReload?.Invoke(gameMenu);
                if (alreadyStarted && gameServer != null)
                    OnServerHotReload?.Invoke(gameServer);
            }
            catch (Exception exception)
            {
                Log.Error($"Error while starting {nameof(PluginLoader)}");
                Sentry.Exception(exception);
            }
        }

        private static void setupLog()
        {
            Log.OnError += message =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{nameof(PluginLoader)}] {message}");
                Console.ResetColor();
            };
            Log.OnException += message =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{nameof(PluginLoader)}] EXCEPTION");
                Console.WriteLine(message);
                Console.ResetColor();
            };
            Log.OnInfo += message => Console.WriteLine($"[{nameof(PluginLoader)}] {message}");
            Log.OnWarning += message =>
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"[{nameof(PluginLoader)}] {message}");
                Console.ResetColor();
            };
        }

        private static void unsubscribeEvent(PLEvent plEvent)
        {
            if (plEvent != null)
                plEvent.GetInvocationList().ToList().ForEach(sub => plEvent -= (PLEvent)sub);
        }

        private void initialize()
        {
            if (initialized)
                return;
            initialized = true;

            Log.Info($"Starting {nameof(PluginLoader)} v{Versions.PLVersion}");

            Serialization.GlobalSerializer.AddTypes(NetSerializable.GetTypesWithAttribute(Assembly.GetExecutingAssembly()));

            resetGameStates();
        }

        private void loadPackages()
        {
            PackageManager.LookForPackages();
            if (PackageManager.Packages.Any())
            {
                Log.Info("Packages:");
                PackageManager.Packages.ForEach(package => Log.Info(package.ToString()));
                solveDependencies();
            }
            else
                Log.Warning("No package found");
        }

        private void resetGameStates()
        {
            gameClient = null;
            gameEditor = null;
            gameMenu = null;
            gameServer = null;
        }

        private void solveDependencies()
        {
            Log.Info("Solving package dependencies");

            UnmetDependency[] unmetDependencies;
            if (!DependencySolver.Solve(PackageManager.Packages.ToArray(), out unmetDependencies))
            {
                unmetDependencies = DependencySolver.FirstLevelUnmetDependencies(unmetDependencies);

                unmetDependencies.ToList().ForEach(unmetDependency =>
                    Log.Error(unmetDependency.ToString()));
                Log.Error("Please resolve issues and restart the game");
                Console.ReadLine();
            }

            try
            {
                PackageManager.Sort();
            }
            catch (Exception exception)
            {
                Log.Error("Unable to sort packages in order to fulfill dependencies");
                Log.Exception(exception);
                Log.Error("Please resolve issues and restart the game");
                Console.ReadLine();
            }
        }
        private void unsubscribeAllEvents()
        {
            unsubscribeEvent(OnAgosGuiInitialized);
            unsubscribeEvent(OnClientHotReload);
            unsubscribeEvent(OnEditorHotReload);
            unsubscribeEvent(OnGameClientActivated);
            unsubscribeEvent(OnGameClientDeactivated);
            unsubscribeEvent(OnGameClientInitialized);
            unsubscribeEvent(OnGameClientUnload);
            unsubscribeEvent(OnGameClientUpdate);
            unsubscribeEvent(OnGameEditorInitialized);
            unsubscribeEvent(OnGameEditorUnload);
            unsubscribeEvent(OnGameEditorUpdate);
            unsubscribeEvent(OnGameMenuInitialized);
            unsubscribeEvent(OnGameServerInitialized);
            unsubscribeEvent(OnGameServerUnload);
            unsubscribeEvent(OnGameServerUpdate);
            unsubscribeEvent(OnMenuHotReload);
            unsubscribeEvent(OnServerHotReload);
        }
    }
}
