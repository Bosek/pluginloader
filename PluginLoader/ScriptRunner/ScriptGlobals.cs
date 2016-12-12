using System;

namespace PluginLoader
{
    public struct ScriptGlobal
    {
        public string GameDirectory { get; set; }
        public string PackageDirectory { get; set; }
        public Package[] Packages { get; set; }
        public PluginManager PluginManager { get; set; }
        public Type Versions { get; set; }
    }
}
