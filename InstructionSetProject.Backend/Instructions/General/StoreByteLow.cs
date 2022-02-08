using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.General
{
    public class StoreByteLow : ImmediateInstruction
    {
        public new const string Mnemonic = "STL";

        public new const ushort OpCode = 0x106;

        public new const bool HighLowBit = false;

        public StoreByteLow(ImmediateInstruction instr)
        {
            base.OpCode = instr.OpCode;
            base.Mnemonic = instr.Mnemonic;
            DestinationRegister = instr.DestinationRegister;
            AddressingMode = instr.AddressingMode;
            base.HighLowBit = instr.HighLowBit;
            Immediate = instr.Immediate;
        }

        public override string GetMnemonic()
        {
            return StoreByteLow.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return StoreByteLow.OpCode;
        }

        public override bool GetHighLowBit()
        {
            return StoreByteLow.HighLowBit;
        }
    }
}
