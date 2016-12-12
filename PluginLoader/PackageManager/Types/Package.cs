using System;

namespace PluginLoader
{
    [Serializable]
    public struct Package
    {
        public Metadata Metadata;
        public string Path;

        public override string ToString()
        {
            return Metadata.ToString();
        }
    }
}
