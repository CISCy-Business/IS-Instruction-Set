using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R1Types
{
    public class PopWord : R1Instruction
    {
        public new const string Mnemonic = "POP";

        public new const ushort OpCode = 0x204;

        public PopWord(R1Instruction instr)
        {
            base.OpCode = instr.OpCode;
            base.Mnemonic = instr.Mnemonic;
            DestinationRegister = instr.DestinationRegister;
            HighLowBit = instr.HighLowBit;
        }

        public override string GetMnemonic()
        {
            return PopWord.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return PopWord.OpCode;
        }

        public override bool GetHighLowBit()
        {
            return false;
        }
    }
}
