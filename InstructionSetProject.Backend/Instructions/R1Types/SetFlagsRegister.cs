using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R1Types
{
    public class SetFlagsRegister : R1Instruction
    {
        public const string Mnemonic = "FLR";

        public const ushort OpCode = 0b0010_0000_0000_1000;

        public override string GetMnemonic()
        {
            return SetFlagsRegister.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return SetFlagsRegister.OpCode;
        }
    }
}
