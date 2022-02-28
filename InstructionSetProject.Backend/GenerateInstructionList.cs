using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend
{
    internal static class GenerateInstructionList
    {
        public static InstructionList FromBytes(List<byte> machineCode)
        {
            var instrList = new InstructionList();

            for (int i = 0; i < machineCode.Count; i += 2)
            {
                var instruction = new List<byte>();
                instruction.Add(machineCode[i]);
                instruction.Add(machineCode[i + 1]);

                var instrType = InstructionUtilities.GetInstructionType(instruction);
                if (instrType == InstructionType.Rm || instrType == InstructionType.Fm)
                {
                    instruction.Add(machineCode[i + 2]);
                    instruction.Add(machineCode[i + 3]);
                    i += 2;
                }

                ushort opcode = (ushort) (instruction[0] << 8 | instruction[1]);
                ushort? operand = (instruction.Count == 2) ? null : (ushort?) (instruction[2] << 8 | instruction[3]);

                var code = (opcode, operand);

                instrList.AddInstruction(InstructionFromBytes(code));
            }

            return instrList;
        }

        private static IInstruction InstructionFromBytes((ushort opcode, ushort? operand) instruction)
        {
            var instr = InstructionManager.Singleton.Get(InstructionUtilities.GetOpCode(instruction.opcode));
            instr.ParseInstruction(instruction);
            return instr;
        }

        private static List<(IInstruction obj, string line)> instrWithLabel = new();

        public static InstructionList FromString(string assemblyCode)
        {
            var assemblyLines = assemblyCode.Split("\n");
            var instrList = new InstructionList();

            foreach (var line in assemblyLines)
            {
                if (line.Trim() != String.Empty && line.Trim().EndsWith(':'))
                {
                    instrList.AddLabel(line.Trim().Trim(':'));
                }
                else if (line.Trim() != String.Empty && !line.Trim().StartsWith('#'))
                {
                    instrList.AddInstruction(InstructionFromString(line.Trim()));
                }
            }

            foreach (var instr in instrWithLabel)
            {
                var lineWithOffset = ReplaceLabelWithOffset(instr.line, instrList);
                instr.obj.ParseInstruction(lineWithOffset);
            }

            instrWithLabel.Clear();

            return instrList;
        }

        private static IInstruction InstructionFromString(string instructionLine)
        {
            var instr = InstructionManager.Singleton.Get(InstructionUtilities.GetMnemonic(instructionLine));
            if (InstructionHasLabel(instr, instructionLine))
            {
                instrWithLabel.Add((instr, instructionLine));
                return instr;
            }
            instr.ParseInstruction(instructionLine);
            return instr;
        }

        private static bool InstructionHasLabel(IInstruction instr, string instructionLine)
        {
            if (instr is RmInstruction intImmInstr)
            {
                return intImmInstr.CheckForLabel(instructionLine);
            }

            if (instr is FmInstruction floatImmInstr)
            {
                return floatImmInstr.CheckForLabel(instructionLine);
            }

            return false;
        }

        private static string ReplaceLabelWithOffset(string instructionLine, InstructionList instrList)
        {
            var tokens = instructionLine.Split(' ');
            for (int i = 1; i < tokens.Length; i++)
            {
                bool hasComma = tokens[i].EndsWith(',');

                var registerRegEx = new Regex("^[RrFf][0-7]$");
                if (registerRegEx.IsMatch(tokens[i].Trim(',')))
                    continue;

                if (UInt16.TryParse(tokens[i].Trim(','), out var result))
                    continue;

                var offset = instrList.GetOffsetFromLabel(tokens[i].Trim(','));

                tokens[i] = offset.ToString("X2");
                if (hasComma) tokens[i] += ",";
            }

            return string.Join(' ', tokens);
        }
    }
}
