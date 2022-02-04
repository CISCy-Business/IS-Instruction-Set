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
        public static string Disassemble(BitArray machineCode)
        {
            var instructionCode = "";
            var boolArr = machineCode.Cast<bool>();
            for (int i = 0; i < machineCode.Count; i += 24)
            {
                var machineLine = ConvertLineToAssemblyCode(boolArr.Skip(i).Take(24));
                
            }
            return instructionCode;
        }

        public static string ConvertLineToAssemblyCode(IEnumerable<bool> machineLine)
        {
            var opCode = GetOpCode(machineLine);

            switch (opCode)
            {
                case Add.OpCode:
                    return Add.Disassemble(machineLine);
                default:
                    throw new Exception("Unable to parse instruction line.");
            }
        }

        private static byte GetOpCode(IEnumerable<bool> machineLine)
        {
            byte result = 0;
            int index = 8 - machineLine.Count();

            foreach (var b in machineLine)
            {
                if (b)
                    result |= (byte) (1 << (7 - index));

                index++;
            }

            return result;
        }
    }
}
