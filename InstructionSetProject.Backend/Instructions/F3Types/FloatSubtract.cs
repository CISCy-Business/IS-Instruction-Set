using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.F3Formats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.F3Types
{
    public class FloatSubtract : F3RegisterRegisterRegister
    {
        public const string Mnemonic = "SUB";

        public const ushort OpCode = 0b0110_0100_0000_0000;

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
