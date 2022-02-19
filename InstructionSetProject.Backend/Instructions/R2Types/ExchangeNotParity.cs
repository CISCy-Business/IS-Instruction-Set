using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R2Types
{
    public class ExchangeNotParity : R2Instruction
    {
        public const string Mnemonic = "XNP";

        public const ushort OpCode = 0b0100_0101_0000_0000;

        public override string GetMnemonic()
        {
            return Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return OpCode;
        }
    }
}
