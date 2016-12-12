using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Launcher.Patches
{
    internal class ProgramPatch : Patch
    {
        public ProgramPatch(PatcherData patcherData) : base(patcherData)
        {
            var irData = patcherData.IRData;
            var plData = patcherData.PLData;
            var irProgramType = irData.Module.GetType("Game.Program");

            var irProgramCtor = CecilHelpers.GetMethodByName(irProgramType.Methods.ToArray(), ".cctor");
            var pluginInjectorType = plData.Module.GetType("PluginLoader.PluginInjector");
            var pluginInjectorCtor = CecilHelpers.GetMethodByName(pluginInjectorType.Methods.ToArray(), ".ctor");
            var irPluginInjectorField = new FieldDefinition("PluginInjector", FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly, irData.Module.Import(pluginInjectorType));
            irProgramType.Fields.Add(irPluginInjectorField);

            var instructions = new List<Instruction>
            {
                Instruction.Create(OpCodes.Ldsfld, CecilHelpers.GetFieldByName(irProgramType.Fields.ToArray(), "log")),
                Instruction.Create(OpCodes.Newobj, irData.Module.Import(pluginInjectorCtor)),
                Instruction.Create(OpCodes.Stsfld, irPluginInjectorField)
            };
            var processor = irProgramCtor.Body.GetILProcessor();
            CecilHelpers.InjectInstructionsToEnd(processor, instructions.ToArray());
        }
    }
}