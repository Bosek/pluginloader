using System;
using Semver;

namespace PluginLoader
{
    [Serializable]
    public struct Dependency
    {
        public string UniqueName;
        public SemVersion Version;

        public override string ToString()
        {
            return $"{UniqueName} v{Version.ToString()}";
        }
    }
}
