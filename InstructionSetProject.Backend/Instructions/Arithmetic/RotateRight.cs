using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.Arithmetic
{
    public class RotateRight : R3Instruction
    {
        public new const string Mnemonic = "RTR";

        public new const ushort OpCode = 0x3A;

        public RotateRight(R3Instruction instr)
        {
            base.OpCode = instr.OpCode;
            base.Mnemonic = instr.Mnemonic;
            DestinationRegister = instr.DestinationRegister;
            SourceRegister1 = instr.SourceRegister1;
            SourceRegister2 = instr.SourceRegister2;
        }

        public override string GetMnemonic()
        {
            return RotateRight.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return RotateRight.OpCode;
        }
    }
}
