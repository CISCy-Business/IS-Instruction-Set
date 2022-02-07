using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.Jump
{
    public class Loop : JumpInstruction
    {
        public new const string Mnemonic = "LOP";

        public new const ushort OpCode = 0x187;

        public Loop(JumpInstruction instr)
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
            return Loop.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return Loop.OpCode;
        }
    }
}
