using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.Stack
{
    public class PopByteLow : R1Instruction
    {
        public new const string Mnemonic = "POPL";

        public new const ushort OpCode = 0x205;

        public new const bool HighLowBit = false;

        public PopByteLow(R1Instruction instr)
        {
            base.OpCode = instr.OpCode;
            base.Mnemonic = instr.Mnemonic;
            DestinationRegister = instr.DestinationRegister;
            base.HighLowBit = instr.HighLowBit;
        }

        public override string GetMnemonic()
        {
            return PopByteLow.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return PopByteLow.OpCode;
        }

        public override bool GetHighLowBit()
        {
            return PopByteLow.HighLowBit;
        }
    }
}
