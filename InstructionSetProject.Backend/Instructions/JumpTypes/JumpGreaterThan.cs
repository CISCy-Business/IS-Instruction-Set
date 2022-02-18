using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.JumpTypes
{
    public class JumpGreaterThan : JumpInstruction
    {
        public new const string Mnemonic = "JGT";

        public new const ushort OpCode = 0x182;

        public JumpGreaterThan(JumpInstruction instr)
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
            return JumpGreaterThan.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return JumpGreaterThan.OpCode;
        }
    }
}
