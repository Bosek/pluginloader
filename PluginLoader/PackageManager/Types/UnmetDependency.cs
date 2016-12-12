using System;

namespace PluginLoader
{
    [Serializable]
    public struct UnmetDependency
    {
        public Dependency Dependency;
        public Package Package;
        public override string ToString()
        {
            return $"{Package.ToString()} is missing dependency {Dependency.ToString()}";
        }
    }
}
