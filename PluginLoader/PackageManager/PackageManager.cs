using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PluginLoader
{
    public class PackageManager
    {
        public string GameDirectory { get; } = AppDomain.CurrentDomain.BaseDirectory;
        public List<Package> Packages { get; private set; } = new List<Package>();
        public string RootDirectory { get; } = Environment.ExpandEnvironmentVariables(@"%appdata%\InterstellarRift\plugins");
        internal void LookForPackages()
        {
            var directories = Directory.GetDirectories(RootDirectory).ToList();

            foreach (string directory in directories)
            {
                try
                {
                    var defFile = Path.Combine(directory, "plugin.json");
                    var metadata = JsonSerializer.Deserialize<Metadata>(File.ReadAllText(defFile));
                    var entryPointPath = Path.Combine(directory, metadata.EntryPoint);

                    if (!File.Exists(entryPointPath))
                        throw new FileNotFoundException(entryPointPath);

                    Packages.Add(new Package
                    {
                        Metadata = metadata,
                        Path = directory
                    });
                }
                catch (Exception exception)
                {
                    Log.Error($"Error loading content of {directory}");
                    Log.Exception(exception);
                    Log.Warning("Directory will be skipped");
                }
            }
        }

        internal void Sort()
        {
            Packages = DependencySolver.Sort(Packages.ToArray()).ToList();
        }
    }
}
