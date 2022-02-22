using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R3Types
{
    public class LogicalShiftLeft : R3Instruction
    {
        public static string Mnemonic = "LSL";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }

        public override ushort GetOpCode()
        {
            // This mnemonic is an alias for ArithmeticShiftLeft. So it returns that Op Code
            return ArithmeticShiftLeft.OpCode;
        }
    }
}
