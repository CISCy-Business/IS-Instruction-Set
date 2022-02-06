using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.Jump
{
    public class JumpLessEqual : JumpInstruction
    {
        public new const string Mnemonic = "JLE";

        public new const ushort OpCode = 0x183;

        public JumpLessEqual(JumpInstruction instr)
        {
            base.OpCode = instr.OpCode;
            base.Mnemonic = instr.Mnemonic;
            DestinationRegister = instr.DestinationRegister;
            HighLowBit = instr.HighLowBit;
            SourceRegister = instr.SourceRegister;
            Immediate = instr.Immediate;
        }

        public override string GetMnemonic()
        {
            return JumpLessThan.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return JumpLessThan.OpCode;
        }
    }
}
