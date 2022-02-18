using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R1Types
{
    public class PushWord : R1Instruction
    {
        public new const string Mnemonic = "PUSH";

        public new const ushort OpCode = 0x202;

        public PushWord(R1Instruction instr)
        {
            base.OpCode = instr.OpCode;
            base.Mnemonic = instr.Mnemonic;
            DestinationRegister = instr.DestinationRegister;
            HighLowBit = instr.HighLowBit;
        }

        public override string GetMnemonic()
        {
            return PushWord.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return PushWord.OpCode;
        }

        public override bool GetHighLowBit()
        {
            return false;
        }
    }
}
