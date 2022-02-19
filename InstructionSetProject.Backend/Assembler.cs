using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Instructions;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend
{
    public static class Assembler
    {
        public static List<byte> Assemble(string assemblyCode)
        {
            var assemblyLines = assemblyCode.Split("\n");
            var machineCode = new List<byte>();
            foreach (var line in assemblyLines)
            {
                if (line != String.Empty)
                {
                    var machineLine = ConvertLineToMachineCode(line);

                    machineCode.Add((byte)(machineLine.opcode >> 8));
                    machineCode.Add((byte)(machineLine.opcode >> 0));

                    if (machineLine.operand != null)
                    {
                        machineCode.Add((byte)(machineLine.operand >> 8));
                        machineCode.Add((byte)(machineLine.operand >> 0));
                    }
                }
            }
            return machineCode;
        }
        
        public static (ushort opcode, ushort? operand) ConvertLineToMachineCode(string instructionLine)
        {
            var instr = InstructionManager.Instance.Get(InstructionUtilities.GetMnemonic(instructionLine));
            instr.ParseInstruction(instructionLine);
            return instr.Assemble();
        }
    }
}
