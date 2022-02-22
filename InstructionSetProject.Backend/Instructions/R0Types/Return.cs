using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Instructions.R0Types;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R0Types
{
    public class Return : R0Instruction
    {
        public const string Mnemonic = "RET";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }

        public override ushort GetOpCode()
        {
            // This mnemonic is an alias for PopPC. So it returns that Op Code
            return PopPC.OpCode;
        }
    }
}
