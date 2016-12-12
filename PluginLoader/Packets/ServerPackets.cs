using Game.Framework;
using Semver;
using System;
using System.Collections.Generic;

namespace PluginLoader.Packets
{
    [NetSerializable]
    [Serializable]
    public struct ServerPLWelcome
    {
        public readonly List<Metadata> Packages;
        public SemVersion PLVersion => Versions.PLVersion;
    }
}
