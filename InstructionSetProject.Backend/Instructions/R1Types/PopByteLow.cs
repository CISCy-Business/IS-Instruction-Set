using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R1Types
{
    public class PopByteLow : R1Instruction
    {
        public const string Mnemonic = "PPL";

        public const ushort OpCode = 0b0010_0000_0011_0000;

        public override string GetMnemonic()
        {
            return PopByteLow.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return PopByteLow.OpCode;
        }
    }
}
