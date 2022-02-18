using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R3Types
{
    public class BitwiseXor : R3Instruction
    {
        public new const string Mnemonic = "XOR";

        public new const ushort OpCode = 0x35;

        public BitwiseXor(R3Instruction instr)
        {
            base.OpCode = instr.OpCode;
            base.Mnemonic = instr.Mnemonic;
            DestinationRegister = instr.DestinationRegister;
            SourceRegister1 = instr.SourceRegister1;
            SourceRegister2 = instr.SourceRegister2;
        }

        public override string GetMnemonic()
        {
            return BitwiseXor.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return BitwiseXor.OpCode;
        }
    }
}
