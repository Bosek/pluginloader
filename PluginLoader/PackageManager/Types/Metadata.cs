using Semver;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PluginLoader
{
    public enum PackageType
    {
        Lua,
        Python,
    }

    [Serializable]
    public struct Metadata
    {
        [JsonProperty(Required = Required.Always)]
        public List<Dependency> Dependencies;
        [JsonProperty(Required = Required.Always)]
        public string EntryPoint;
        [JsonProperty(Required = Required.Always)]
        public SemVersion PLVersion;
        [JsonProperty(Required = Required.Always)]
        public string PrettyName;
        [JsonProperty(Required = Required.Always)]
        public PackageType Type;
        [JsonProperty(Required = Required.Always)]
        public string UniqueName;
        [JsonProperty(Required = Required.Always)]
        public SemVersion Version;
        public override string ToString()
        {
            return $"{UniqueName} v{Version.ToString()} {Type.ToString().ToUpper()}";
        }
    }
}
