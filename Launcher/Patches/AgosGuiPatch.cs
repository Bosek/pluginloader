using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Launcher.Patches
{
    internal class AgosGuiPatch : Patch
    {
        public AgosGuiPatch(PatcherData patcherData) : base(patcherData)
        {
            var irData = patcherData.IRData;
            var plData = patcherData.PLData;
            var irProgramType = irData.Module.GetType("Game.Program");
            var pluginInjectorType = plData.Module.GetType("PluginLoader.PluginInjector");
            var irPluginInjectorField = CecilHelpers.GetFieldByName(irProgramType.Fields.ToArray(), "PluginInjector");
            var irAgosGuiType = irData.Module.GetType("Game.Client.AgosGui");

            var instructions = new List<Instruction>
            {
                Instruction.Create(OpCodes.Ldsfld, irPluginInjectorField),
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Callvirt, irData.Module.Import(CecilHelpers.GetMethodByName(pluginInjectorType.Methods.ToArray(), "IRInitializeAgosGui")))
            };
            var processor = CecilHelpers.GetMethodByName(irAgosGuiType.Methods.ToArray(), ".ctor").Body.GetILProcessor();
            CecilHelpers.InjectInstructionsToEnd(processor, instructions.ToArray());
        }
    }
}