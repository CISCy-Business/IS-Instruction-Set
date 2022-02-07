using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.General
{
    public class StoreByteHigh : ImmediateInstruction
    {
        public new const string Mnemonic = "STH";

        public new const ushort OpCode = 0x83;

        public new const bool HighLowBit = true;

        public StoreByteHigh(ImmediateInstruction instr)
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
            return StoreByteHigh.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return StoreByteHigh.OpCode;
        }
    }
}
