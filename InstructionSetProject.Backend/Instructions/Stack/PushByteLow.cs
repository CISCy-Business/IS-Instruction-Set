using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.Stack
{
    public class PushByteLow : R1Instruction
    {
        public new const string Mnemonic = "PSHL";

        public new const ushort OpCode = 0x203;

        public new const bool HighLowBit = false;

        public PushByteLow(R1Instruction instr)
        {
            base.OpCode = instr.OpCode;
            base.Mnemonic = instr.Mnemonic;
            DestinationRegister = instr.DestinationRegister;
            base.HighLowBit = instr.HighLowBit;
        }

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
