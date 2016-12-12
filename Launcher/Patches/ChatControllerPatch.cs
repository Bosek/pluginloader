using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

namespace Launcher.Patches
{
    internal class ChatControllerPatch : Patch
    {
        public ChatControllerPatch(PatcherData patcherData) : base(patcherData)
        {
            var irData = patcherData.IRData;
            var plData = patcherData.PLData;
            var irChatControllerType = irData.Module.GetType("Game.Server.ChatController");
            var instructions = new List<Instruction>();

            var stringType = irData.Module.Import(typeof(string));
            var actionType = irData.Module.Import(typeof(Action<,>));
            var dictionaryType = irData.Module.Import(typeof(Dictionary<,>));
            var irPlayerType = irData.Module.GetType("Game.Server.Player");
            var actionInstanceType = new GenericInstanceType(actionType);
            actionInstanceType.GenericArguments.Add(irPlayerType);
            actionInstanceType.GenericArguments.Add(stringType);
            var dictionaryInstanceType = new GenericInstanceType(dictionaryType);
            dictionaryInstanceType.GenericArguments.Add(stringType);
            dictionaryInstanceType.GenericArguments.Add(actionInstanceType);


            var getCommandsMethod = new MethodDefinition("get_Commands", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, dictionaryInstanceType);
            irChatControllerType.Methods.Add(getCommandsMethod);
            irChatControllerType.Properties.Add(new PropertyDefinition("Commands", PropertyAttributes.None, dictionaryInstanceType)
            {
                GetMethod = getCommandsMethod
            });

            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Ldfld, CecilHelpers.GetFieldByName(irChatControllerType.Fields.ToArray(), "m_commands")));
            instructions.Add(Instruction.Create(OpCodes.Ret));
            var processor = getCommandsMethod.Body.GetILProcessor();
            instructions.ForEach(instruction => processor.Append(instruction));
        }

    }
}