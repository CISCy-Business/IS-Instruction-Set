using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.RmFormats;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.Instructions.RmTypes
{
    public class SetMemoryBasePointer : RmImmediate
    {
        public const string Mnemonic = "SMP";

        public const ushort OpCode = 0b1101_0000_0000_0000;

        public override RegisterType? firstRegisterType => RegisterType.Write;

        public override ushort? firstRegister { get => 0b1000; set { } }

        public override ControlBits controlBits => new(true, true, false, false, false, false, false);

        public override AluOperation? aluOperation => AluOperation.PassSecondOperandThrough;

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
