using Game.Networking;
using System.IO;

namespace PluginLoader
{
    abstract class GameMode
    {
        protected readonly PluginManager pluginManager;
        protected GameMode(PluginManager pluginManager)
        {
            this.pluginManager = pluginManager;
        }

        public static string AddFileToFS(NetFilesystem filesystem, string path, string filename, byte[] data)
        {
            var key = $"{path}{filename}";

            filesystem.FileSystem.WriteFile(path, filename, data);
            var netSource = new NetFilestreamSource
            {
                Path = path,
                Filename = filename,
            };
            filesystem.IdentifierSources.Add(key, netSource);
            return key;
        }

        public static Stream LoadFileFromFS(NetFilesystem filesystem, string path, string filename)
        {
            return filesystem.FileSystem.ReadFile(path, filename);
        }

        public virtual void StartGameMode()
        {
            Initialize();
        }

        private void Initialize()
        {
            Log.Info($"Starting {GetType().Name} mode");
        }
    }
}