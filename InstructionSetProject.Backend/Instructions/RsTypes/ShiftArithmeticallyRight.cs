using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.RsFormats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.RsTypes
{
    public class ShiftArithmeticallyRight : RsRegisterRegisterImmediate
    {
        public const string Mnemonic = "SAR";

        public const ushort OpCode = 0b1000_1100_0000_0000;

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
