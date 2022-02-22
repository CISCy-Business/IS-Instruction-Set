using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R0Types
{
    public class PopPC : R0Instruction
    {
        public const string Mnemonic = "PPPC";

        public const ushort OpCode = 0b0000_0000_0001_1000;

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
