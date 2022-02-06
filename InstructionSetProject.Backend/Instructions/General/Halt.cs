using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.General
{
    public class Halt : R0Instruction
    {
        public new const string Mnemonic = "HLT";

        public new const ushort OpCode = 0x0;

        public Halt(R0Instruction instr)
        {
            base.OpCode = instr.OpCode;
            base.Mnemonic = instr.Mnemonic;
        }

        public override string GetMnemonic()
        {
            return Halt.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return Halt.OpCode;
        }
    }
}
