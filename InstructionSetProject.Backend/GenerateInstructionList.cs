using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend
{
    internal static class GenerateInstructionList
    {
        public static List<IInstruction> FromBytes(List<byte> machineCode)
        {
            var instructions = new List<IInstruction>();

            for (int i = 0; i < machineCode.Count; i += 2)
            {
                var instruction = new List<byte>();
                instruction.Add(machineCode[i]);
                instruction.Add(machineCode[i + 1]);

                var instrType = InstructionUtilities.GetInstructionType(instruction);
                if (instrType == InstructionType.Memory || instrType == InstructionType.Jump ||
                    instrType == InstructionType.R2I)
                {
                    instruction.Add(machineCode[i + 2]);
                    instruction.Add(machineCode[i + 3]);
                    i += 2;
                }

                ushort opcode = (ushort) (instruction[0] << 8 | instruction[1]);
                ushort? operand = (instruction.Count == 2) ? null : (ushort?) (instruction[2] << 8 | instruction[3]);

                var code = (opcode, operand);

                instructions.Add(InstructionFromBytes(code));
            }

            return instructions;
        }

        private static IInstruction InstructionFromBytes((ushort opcode, ushort? operand) instruction)
        {
            var instr = InstructionManager.Instance.Get(InstructionUtilities.GetOpCode(instruction.opcode));
            instr.ParseInstruction(instruction);
            return instr;
        }

        public static List<IInstruction> FromString(string assemblyCode)
        {
            var assemblyLines = assemblyCode.Split("\n");
            var instructions = new List<IInstruction>();

            foreach (var line in assemblyLines)
            {
                if (line.Trim() != String.Empty && !line.Trim().StartsWith('#'))
                {
                    instructions.Add(InstructionFromString(line));
                }
            }

            return instructions;
        }

        private static IInstruction InstructionFromString(string instructionLine)
        {
            var instr = InstructionManager.Instance.Get(InstructionUtilities.GetMnemonic(instructionLine));
            instr.ParseInstruction(instructionLine);
            return instr;
        }
    }
}
