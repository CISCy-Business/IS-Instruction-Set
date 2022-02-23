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
    public static class Assembler
    {
        public static List<byte> Assemble(string assemblyCode)
        {
            var instructions = GenerateInstructionList.FromString(assemblyCode);
            var machineCode = new List<byte>();
            foreach (var instr in instructions)
            {
                var machineLine = AssembleInstruction(instr);

                foreach (var codePiece in machineLine)
                {
                    machineCode.Add((byte) (codePiece >> 8));
                    machineCode.Add((byte) (codePiece >> 0));
                }
            }
            return machineCode;
        }
        
        public static List<ushort> AssembleInstruction(IInstruction instr)
        {
            if (instr is ICISCInstruction)
            {
                return ((ICISCInstruction) instr).CISCAssemble();
            }

            var normalAssembly = instr.Assemble();
            var assemblyReturn = new List<ushort>() { normalAssembly.opcode };
            if (normalAssembly.operand != null)
                assemblyReturn.Add((ushort)normalAssembly.operand);
            return assemblyReturn;
        }
    }
}
