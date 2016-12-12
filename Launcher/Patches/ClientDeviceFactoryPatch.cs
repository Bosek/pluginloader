using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;

namespace Launcher.Patches
{
    internal class ClientDeviceFactoryPatch : Patch
    {
        public ClientDeviceFactoryPatch(PatcherData patcherData) : base(patcherData)
        {
            var irData = patcherData.IRData;
            var plData = patcherData.PLData;
            var irProgramType = irData.Module.GetType("Game.Program");
            var pluginInjectorType = plData.Module.GetType("PluginLoader.PluginInjector");
            var irPluginInjectorField = CecilHelpers.GetFieldByName(irProgramType.Fields.ToArray(), "PluginInjector");
            var irFactoryType = irData.Module.GetType("Game.Ship.Lockstep.ClientState.Factory");

            var instructions = new List<Instruction>();
            instructions.Add(Instruction.Create(OpCodes.Ldsfld, irPluginInjectorField));
            instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
            instructions.Add(Instruction.Create(OpCodes.Callvirt, irData.Module.Import(CecilHelpers.GetMethodByName(pluginInjectorType.Methods.ToArray(), "IRCreateClientDevice"))));
            var processor = irFactoryType.Methods.First(method => method.Name == "CreateFrom").Body.GetILProcessor();
            processor.Body.Instructions.Remove(processor.Body.Instructions.Last().Previous);
            CecilHelpers.InjectInstructionsToEnd(processor, instructions.ToArray());

            instructions.Clear();
            instructions.Add(Instruction.Create(OpCodes.Ldsfld, irPluginInjectorField));
            instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
            instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
            instructions.Add(Instruction.Create(OpCodes.Callvirt, irData.Module.Import(CecilHelpers.GetMethodByName(pluginInjectorType.Methods.ToArray(), "IRCreateClientDeviceExterior"))));
            processor = irFactoryType.Methods.Last(method => method.Name == "CreateFrom").Body.GetILProcessor();
            CecilHelpers.InjectInstructionsToEnd(processor, instructions.ToArray());
        }
    }
}