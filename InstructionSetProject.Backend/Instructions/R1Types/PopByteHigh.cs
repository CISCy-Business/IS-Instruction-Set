using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R1Types
{
    public class PopByteHigh : R1Instruction
    {
        public new const string Mnemonic = "POPH";

        public new const ushort OpCode = 0x205;

        public new const bool HighLowBit = true;

        public PopByteHigh(R1Instruction instr)
        {
            base.OpCode = instr.OpCode;
            base.Mnemonic = instr.Mnemonic;
            DestinationRegister = instr.DestinationRegister;
            base.HighLowBit = instr.HighLowBit;
        }

        public override string GetMnemonic()
        {
            return PopByteHigh.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return PopByteHigh.OpCode;
        }

        public override bool GetHighLowBit()
        {
            return PopByteHigh.HighLowBit;
        }
    }
}
