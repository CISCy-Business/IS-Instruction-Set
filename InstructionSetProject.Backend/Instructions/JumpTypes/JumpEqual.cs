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
        public const string Mnemonic = "JEQ";

        public const ushort OpCode = 0b1010_0000_0100_0000;

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
