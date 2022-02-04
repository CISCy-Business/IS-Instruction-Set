using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Instructions;

namespace InstructionSetProject.Backend
{
    internal static class Disassembler
    {
        public static string Disassemble(List<byte> machineCode)
        {
            var instructionCode = "";
            
            // Implement logic for dynamic instruction length

            return instructionCode.TrimEnd();
        }

        public static string ConvertLineToAssemblyCode(List<byte> machineLine)
        {
            var opCode = GetOpCode(machineLine);

            var instr = GetInstruction.FromOpCode(opCode);

            return instr.Disassemble(machineLine);
        }

        private static byte GetOpCode(List<byte> machineLine)
        {
            throw new NotImplementedException();
        }
    }
}
