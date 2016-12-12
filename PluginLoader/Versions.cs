using Semver;

namespace PluginLoader
{
    public static class Versions
    {
        public static string IRVersion { get; } = Game.Configuration.Globals.Version;

        public static SemVersion PLVersion { get; } = new SemVersion(0, 11, 0, "alpha");
    }
}
