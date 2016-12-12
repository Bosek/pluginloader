using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Launcher.Patches
{
    internal class GameServerPatch : Patch
    {
        public GameServerPatch(PatcherData patcherData) : base(patcherData)
        {
            var irData = patcherData.IRData;
            var plData = patcherData.PLData;
            var irProgramType = irData.Module.GetType("Game.Program");
            var pluginInjectorType = plData.Module.GetType("PluginLoader.PluginInjector");

            var irPluginInjectorField = CecilHelpers.GetFieldByName(irProgramType.Fields.ToArray(), "PluginInjector");
            var irGameServerType = irData.Module.GetType("Game.GameStates.GameServer");
            var instructions = new List<Instruction>
            {
                Instruction.Create(OpCodes.Ldsfld, irPluginInjectorField),
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Callvirt, irData.Module.Import(CecilHelpers.GetMethodByName(pluginInjectorType.Methods.ToArray(), "IRInitializeGameServer")))
            };
            var processor = CecilHelpers.GetMethodByName(irGameServerType.Methods.ToArray(), ".ctor").Body.GetILProcessor();
            CecilHelpers.InjectInstructionsToEnd(processor, instructions.ToArray());

            instructions.Clear();
            instructions.Add(Instruction.Create(OpCodes.Ldsfld, irPluginInjectorField));
            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Callvirt, CecilHelpers.GetMethodByName(irGameServerType.Methods.ToArray(), "get_TickrateInSeconds")));
            instructions.Add(Instruction.Create(OpCodes.Callvirt, irData.Module.Import(CecilHelpers.GetMethodByName(pluginInjectorType.Methods.ToArray(), "IRUpdateGameServer"))));
            processor = CecilHelpers.GetMethodByName(irGameServerType.Methods.ToArray(), "UpdateOffthread").Body.GetILProcessor();
            CecilHelpers.InjectInstructionsToEnd(processor, instructions.ToArray());

            instructions.Clear();
            instructions.Add(Instruction.Create(OpCodes.Ldsfld, irPluginInjectorField));
            instructions.Add(Instruction.Create(OpCodes.Callvirt, irData.Module.Import(CecilHelpers.GetMethodByName(pluginInjectorType.Methods.ToArray(), "IRUnloadGameServer"))));
            processor = CecilHelpers.GetMethodByName(irGameServerType.Methods.ToArray(), "Unload").Body.GetILProcessor();
            CecilHelpers.InjectInstructionsToEnd(processor, instructions.ToArray());
        }
    }
}