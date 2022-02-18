using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R0Types
{
    public class Halt : R0Instruction
    {
        public const string Mnemonic = "HLT";

        public const ushort OpCode = 0b0000_0000_0000_0000;

        public override string GetMnemonic()
        {
            return Halt.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return Halt.OpCode;
        }
    }
}
