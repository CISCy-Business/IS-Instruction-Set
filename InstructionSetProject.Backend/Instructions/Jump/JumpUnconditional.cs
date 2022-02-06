using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.Jump
{
    public class JumpUnconditional : JumpInstruction
    {
        public new const string Mnemonic = "JMP";

        public new const ushort OpCode = 0x180;

        public JumpUnconditional(JumpInstruction instr)
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
            return JumpUnconditional.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return JumpUnconditional.OpCode;
        }
    }
}
