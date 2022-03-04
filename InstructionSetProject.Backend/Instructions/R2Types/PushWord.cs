using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.R2Formats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.R2Types
{
    public class PushWord : R2Register
    {
        public const string Mnemonic = "PSW";

        public const ushort OpCode = 0b0000_0001_0000_0000;

        public override ControlBits controlBits => throw new NotImplementedException();

        public override AluOperation? aluOperation => null;

        public override string GetMnemonic()
        {
            return Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return OpCode;
        }
    }
}
