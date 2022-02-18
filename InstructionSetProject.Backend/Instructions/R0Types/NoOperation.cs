using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R0Types
{
    public class NoOperation : R0Instruction
    {
        public new const string Mnemonic = "NOP";

        public new const ushort OpCode = 0x1;

        public NoOperation(R0Instruction instr)
        {
            base.OpCode = instr.OpCode;
            base.Mnemonic = instr.Mnemonic;
        }

        public override string GetMnemonic()
        {
            return NoOperation.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return NoOperation.OpCode;
        }
    }
}
