using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.Arithmetic
{
    public class BitwiseNot : R2Instruction
    {
        public new const string Mnemonic = "NOT";

        public new const ushort OpCode = 0x100;

        public BitwiseNot(R2Instruction instr)
        {
            base.OpCode = instr.OpCode;
            base.Mnemonic = instr.Mnemonic;
            DestinationRegister = instr.DestinationRegister;
            SourceRegister = instr.SourceRegister;
        }

        public override string GetMnemonic()
        {
            return BitwiseNot.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return BitwiseNot.OpCode;
        }
    }
}
