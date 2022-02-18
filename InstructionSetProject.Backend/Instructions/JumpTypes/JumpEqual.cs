using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.JumpTypes
{
    public class JumpEqual : JumpInstruction
    {
        public new const string Mnemonic = "JEQ";

        public new const ushort OpCode = 0x185;

        public JumpEqual(JumpInstruction instr)
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
            return JumpEqual.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return JumpEqual.OpCode;
        }
    }
}
