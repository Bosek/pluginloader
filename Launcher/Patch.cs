using Mono.Cecil;
using System.Reflection;

namespace Launcher
{
    enum PatcherTarget
    {
        InterstellarRift,
        PluginLoader
    }
    struct PatcherTargetData
    {
        public Assembly Assembly;
        public AssemblyDefinition AssemblyDef;
        public ModuleDefinition Module;
        public string Path;
    }
    abstract class Patch
    {
        protected readonly PatcherData patcherData;

        protected Patch(PatcherData patcherData)
        {
            this.patcherData = patcherData;
        }
    }

    class PatcherData
    {
        public PatcherTargetData IRData { get; private set; }
        public PatcherTargetData PLData { get; private set; }

        public void Load(PatcherTargetData irData, PatcherTargetData plData)
        {
            if (IRData.Equals(default(PatcherTargetData)))
                IRData = irData;
            if (PLData.Equals(default(PatcherTargetData)))
                PLData = plData;
        }
    }
}
