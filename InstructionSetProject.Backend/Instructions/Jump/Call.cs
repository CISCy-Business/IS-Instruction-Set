using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.Jump
{
    public class Call : JumpInstruction
    {
        public new const string Mnemonic = "CAL";

        public new const ushort OpCode = 0x1C0;

        public Call(JumpInstruction instr)
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
            return Call.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return Call.OpCode;
        }
    }
}
