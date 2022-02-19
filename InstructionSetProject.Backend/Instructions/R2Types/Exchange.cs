using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R2Types
{
    public class Exchange : R2Instruction
    {
        public const string Mnemonic = "XCH";

        public const ushort OpCode = 0b0100_0000_0000_0000;

        public override string GetMnemonic()
        {
            return Exchange.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return Exchange.OpCode;
        }
    }
}
