using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R2ITypes
{
    public class BitwiseSubtractImmediateBorrow : R2IInstruction
    {
        public const string Mnemonic = "SIB";

        public const ushort OpCode = 0b1100_0000_1100_0000;

        public override string GetMnemonic()
        {
            return BitwiseSubtractImmediateBorrow.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return BitwiseSubtractImmediateBorrow.OpCode;
        }
    }
}
