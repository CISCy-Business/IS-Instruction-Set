using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R2ITypes
{
    public class BitwiseAndImmediate : R2IInstruction
    {
        public const string Mnemonic = "ANI";

        public const ushort OpCode = 0b1100_0001_0000_0000;

        public override string GetMnemonic()
        {
            return BitwiseAndImmediate.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return BitwiseAndImmediate.OpCode;
        }
    }
}
