using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Instructions;
using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend
{
    public static class Disassembler
    {
        public static string Disassemble(List<byte> machineCode)
        {
            var instructionCode = "";

            for (int i = 0; i < machineCode.Count; i += 2)
            {
                var instruction = new List<byte>();
                instruction.Add(machineCode[i]);
                instruction.Add(machineCode[i + 1]);
                var instrType = InstructionUtilities.GetInstructionType(instruction);
                if (instrType == InstructionType.Immediate || instrType == InstructionType.Jump)
                {
                    instruction.Add(machineCode[i + 2]);
                    instruction.Add(machineCode[i + 3]);
                    i += 2;
                }

                instructionCode += ConvertLineToAssemblyCode(instruction, instrType);
                instructionCode += "\n";
            }

            return instructionCode.TrimEnd();
        }

        public static string ConvertLineToAssemblyCode(List<byte> instruction, InstructionType type)
        {
            IInstruction genericTypedInstr, typedInstr;
            switch (type)
            {
                case InstructionType.R0:
                    genericTypedInstr = R0Instruction.ParseInstruction(instruction);
                    typedInstr = GetInstruction.FromOpCode(genericTypedInstr);
                    return typedInstr.Disassemble();
                case InstructionType.R1:
                    genericTypedInstr = R1Instruction.ParseInstruction(instruction);
                    typedInstr = GetInstruction.FromOpCode(genericTypedInstr);
                    return typedInstr.Disassemble();
                case InstructionType.R2:
                    genericTypedInstr = R2Instruction.ParseInstruction(instruction);
                    typedInstr = GetInstruction.FromOpCode(genericTypedInstr);
                    return typedInstr.Disassemble();
                case InstructionType.R3:
                    genericTypedInstr = R3Instruction.ParseInstruction(instruction);
                    typedInstr = GetInstruction.FromOpCode(genericTypedInstr);
                    return typedInstr.Disassemble();
                case InstructionType.Immediate:
                    genericTypedInstr = ImmediateInstruction.ParseInstruction(instruction);
                    typedInstr = GetInstruction.FromOpCode(genericTypedInstr);
                    return typedInstr.Disassemble();
                case InstructionType.Jump:
                    genericTypedInstr = JumpInstruction.ParseInstruction(instruction);
                    typedInstr = GetInstruction.FromOpCode(genericTypedInstr);
                    return typedInstr.Disassemble();
                default:
                    throw new Exception("Instruction type not found");
            }
        }
    }
}
