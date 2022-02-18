using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R2ITypes
{
    internal class BitwiseAddImmediate : R2IInstruction
    {
        public const string Mnemonic = "ADI";

        public const ushort OpCode = 0b1100_0000_0000_0000;

        public override string GetMnemonic()
        {
            return BitwiseAddImmediate.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return BitwiseAddImmediate.OpCode;
        }
    }
}
