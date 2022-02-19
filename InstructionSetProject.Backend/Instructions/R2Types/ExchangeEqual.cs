using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R2Types
{
    public class ExchangeEqual : R2Instruction
    {
        public const string Mnemonic = "XEQ";

        public const ushort OpCode = 0b0100_0000_0100_0000;

        public override string GetMnemonic()
        {
            return ExchangeEqual.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return ExchangeEqual.OpCode;
        }
    }
}
