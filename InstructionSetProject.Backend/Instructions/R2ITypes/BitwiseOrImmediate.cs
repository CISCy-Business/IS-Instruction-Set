using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R2ITypes
{
    public class BitwiseOrImmediate : R2IInstruction
    {
        public const string Mnemonic = "ORI";

        public const ushort OpCode = 0b1100_0001_0100_0000;

        public override string GetMnemonic()
        {
            return BitwiseOrImmediate.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return BitwiseOrImmediate.OpCode;
        }
    }
}
