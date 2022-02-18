using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R0Types
{
    public class NoOperation : R0Instruction
    {
        public const string Mnemonic = "NOP";

        public const ushort OpCode = 0b0000_0000_0000_1000;

        public override string GetMnemonic()
        {
            return NoOperation.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return NoOperation.OpCode;
        }
    }
}
