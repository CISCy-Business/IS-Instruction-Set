using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R2ITypes
{
    public class BitwiseSubtractImmediate : R2IInstruction
    {
        public const string Mnemonic = "SBI";

        public const ushort OpCode = 0b1100_0000_0100_0000;

        public override string GetMnemonic()
        {
            return BitwiseSubtractImmediate.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return BitwiseSubtractImmediate.OpCode;
        }
    }
}
