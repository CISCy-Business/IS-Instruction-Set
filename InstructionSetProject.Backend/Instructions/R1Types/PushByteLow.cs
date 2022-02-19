using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R1Types
{
    public class PushByteLow : R1Instruction
    {
        public const string Mnemonic = "PSL";

        public const ushort OpCode = 0b0010_0000_0001_1000;

        public override string GetMnemonic()
        {
            return PushByteLow.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return PushByteLow.OpCode;
        }
    }
}
