using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R1Types
{
    public class SetFlagsExplicit : R1Instruction
    {
        public const string Mnemonic = "FLG";

        public const ushort OpCode = 0b0010_0000_0000_0000;

        public override string GetMnemonic()
        {
            return SetFlagsExplicit.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return SetFlagsExplicit.OpCode;
        }
    }
}
