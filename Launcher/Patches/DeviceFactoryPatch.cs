using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;

namespace Launcher.Patches
{
    internal class DeviceFactoryPatch : Patch
    {
        public DeviceFactoryPatch(PatcherData patcherData) : base(patcherData)
        {
            var irData = patcherData.IRData;
            var plData = patcherData.PLData;
            var irProgramType = irData.Module.GetType("Game.Program");
            var pluginInjectorType = plData.Module.GetType("PluginLoader.PluginInjector");
            var irPluginInjectorField = CecilHelpers.GetFieldByName(irProgramType.Fields.ToArray(), "PluginInjector");
            var irFactoryType = irData.Module.GetType("Game.Ship.Lockstep.State.Factory");

            var instructions = new List<Instruction>
            {
                Instruction.Create(OpCodes.Ldsfld, irPluginInjectorField),
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Callvirt, irData.Module.Import(CecilHelpers.GetMethodByName(pluginInjectorType.Methods.ToArray(), "IRCreateDevice")))
            };
            var processor = CecilHelpers.GetMethodByName(irFactoryType.Methods.ToArray(), "Create").Body.GetILProcessor();
            processor.Remove(processor.Body.Instructions.Last().Previous);
            CecilHelpers.InjectInstructionsToEnd(processor, instructions.ToArray());
        }
    }
}