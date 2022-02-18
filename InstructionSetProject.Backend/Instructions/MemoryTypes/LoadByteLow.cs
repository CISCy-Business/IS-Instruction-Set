using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.MemoryTypes
{
    public class LoadByteLow : MemoryInstruction
    {
        public new const string Mnemonic = "LDL";

        public new const ushort OpCode = 0x81;

        public new const bool HighLowBit = false;

        public LoadByteLow(MemoryInstruction instr)
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
            return LoadByteLow.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return LoadByteLow.OpCode;
        }

        public override bool GetHighLowBit()
        {
            return LoadByteLow.HighLowBit;
        }
    }
}
