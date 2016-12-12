using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Launcher.Patches
{
    internal class GameMenuPatch : Patch
    {
        public GameMenuPatch(PatcherData patcherData) : base(patcherData)
        {
            var irData = patcherData.IRData;
            var plData = patcherData.PLData;
            var irProgramType = irData.Module.GetType("Game.Program");
            var pluginInjectorType = plData.Module.GetType("PluginLoader.PluginInjector");
            var irPluginInjectorField = CecilHelpers.GetFieldByName(irProgramType.Fields.ToArray(), "PluginInjector");
            var irGameClientType = irData.Module.GetType("Game.GameStates.GameMainMenu");

            var instructions = new List<Instruction>
            {
                Instruction.Create(OpCodes.Ldsfld, irPluginInjectorField),
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Callvirt, irData.Module.Import(CecilHelpers.GetMethodByName(pluginInjectorType.Methods.ToArray(), "IRInitializeGameMenu")))
            };
            var processor = CecilHelpers.GetMethodByName(irGameClientType.Methods.ToArray(), ".ctor").Body.GetILProcessor();
            CecilHelpers.InjectInstructionsToEnd(processor, instructions.ToArray());
        }
    }
}