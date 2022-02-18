using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R1Types
{
    public class PushByteHigh : R1Instruction
    {
        public new const string Mnemonic = "PSHH";

        public new const ushort OpCode = 0x203;

        public new const bool HighLowBit = true;

        public PushByteHigh(R1Instruction instr)
        {
            base.OpCode = instr.OpCode;
            base.Mnemonic = instr.Mnemonic;
            DestinationRegister = instr.DestinationRegister;
            base.HighLowBit = instr.HighLowBit;
        }

        public override string GetMnemonic()
        {
            return PushByteHigh.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return PushByteHigh.OpCode;
        }

        public override bool GetHighLowBit()
        {
            return PushByteHigh.HighLowBit;
        }
    }
}
