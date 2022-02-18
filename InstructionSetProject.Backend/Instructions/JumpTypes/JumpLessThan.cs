using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.JumpTypes
{
    public class JumpLessThan : JumpInstruction
    {
        public new const string Mnemonic = "JLT";

        public new const ushort OpCode = 0x181;

        public JumpLessThan(JumpInstruction instr)
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
