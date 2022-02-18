using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R2Types
{
    public class BitwiseNeg : R2Instruction
    {
        public const string Mnemonic = "NEG";

        public const ushort OpCode = 0b0100_0101_1000_0000;

        public override string GetMnemonic()
        {
            return BitwiseNeg.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return BitwiseNeg.OpCode;
        }
    }
}
