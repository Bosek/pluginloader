using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace Launcher
{
    static class CecilHelpers
    {

        public static FieldDefinition GetFieldByName(FieldDefinition[] fields, string name)
        {
            return fields.Single(field => field.Name == name);
        }

        public static Instruction GetInstructionByOffset(MethodBody method, int offset)
        {
            return method.Instructions.Single(instruction => instruction.Offset == offset);
        }

        public static MethodDefinition GetMethodByName(MethodDefinition[] methods, string name)
        {
            return methods.Single(method => method.Name == name);
        }

        public static void InjectInstructionsAfter(ILProcessor processor, Instruction[] instructions, int offset)
        {
            var lastInstruction = GetInstructionByOffset(processor.Body, offset);
            foreach (Instruction instruction in instructions)
            {
                processor.InsertAfter(lastInstruction, instruction);
                lastInstruction = instruction;
            }
        }

        public static void InjectInstructionsBefore(ILProcessor processor, Instruction[] instructions, int offset)
        {
            Instruction lastInstruction = null;
            foreach (Instruction instruction in instructions)
            {
                if(lastInstruction == null)
                {
                    processor.InsertBefore(GetInstructionByOffset(processor.Body, offset), instruction);
                }
                else
                {
                    processor.InsertAfter(lastInstruction, instruction);
                }
                lastInstruction = instruction;
            }
        }

        public static void InjectInstructionsToEnd(ILProcessor processor, Instruction[] instructions)
        {
            var lastInstruction = processor.Body.Instructions.Last();
            processor.Replace(lastInstruction, instructions.First());

            if (instructions.Length > 0)
                instructions.ToList().GetRange(1, instructions.Length - 1).ForEach(instruction => processor.Append(instruction));
            processor.Append(Instruction.Create(OpCodes.Ret));
        }
    }
}
