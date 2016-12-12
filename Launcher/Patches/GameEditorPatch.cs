using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Launcher.Patches
{
    internal class GameEditorPatch : Patch
    {
        public GameEditorPatch(PatcherData patcherData) : base(patcherData)
        {
            var irData = patcherData.IRData;
            var plData = patcherData.PLData;
            var irProgramType = irData.Module.GetType("Game.Program");
            var pluginInjectorType = plData.Module.GetType("PluginLoader.PluginInjector");
            var irPluginInjectorField = CecilHelpers.GetFieldByName(irProgramType.Fields.ToArray(), "PluginInjector");
            var irGameClientType = irData.Module.GetType("Game.GameStates.GameShipEditor");
            var instructions = new List<Instruction>();

            instructions.Add(Instruction.Create(OpCodes.Ldsfld, irPluginInjectorField));
            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Callvirt, irData.Module.Import(CecilHelpers.GetMethodByName(pluginInjectorType.Methods.ToArray(), "IRInitializeGameEditor"))));
            var processor = CecilHelpers.GetMethodByName(irGameClientType.Methods.ToArray(), ".ctor").Body.GetILProcessor();
            CecilHelpers.InjectInstructionsToEnd(processor, instructions.ToArray());

            instructions.Clear();
            instructions.Add(Instruction.Create(OpCodes.Ldsfld, irPluginInjectorField));
            instructions.Add(Instruction.Create(OpCodes.Ldc_R4, 0.01666667f));
            instructions.Add(Instruction.Create(OpCodes.Callvirt, irData.Module.Import(CecilHelpers.GetMethodByName(pluginInjectorType.Methods.ToArray(), "IRUpdateGameEditor"))));
            processor = CecilHelpers.GetMethodByName(irGameClientType.Methods.ToArray(), "UpdateFixedStep").Body.GetILProcessor();
            CecilHelpers.InjectInstructionsToEnd(processor, instructions.ToArray());

            instructions.Clear();
            instructions.Add(Instruction.Create(OpCodes.Ldsfld, irPluginInjectorField));
            instructions.Add(Instruction.Create(OpCodes.Callvirt, irData.Module.Import(CecilHelpers.GetMethodByName(pluginInjectorType.Methods.ToArray(), "IRUnloadGameEditor"))));
            processor = CecilHelpers.GetMethodByName(irGameClientType.Methods.ToArray(), "Unload").Body.GetILProcessor();
            CecilHelpers.InjectInstructionsToEnd(processor, instructions.ToArray());
        }
    }
}