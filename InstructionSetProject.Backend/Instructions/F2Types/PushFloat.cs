using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.F2Formats;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.Instructions.F2Types
{
    public class PushFloat : F2Register
    {
        public const string Mnemonic = "PSF";

        public const ushort OpCode = 0b0010_0001_0000_0000;

        public override RegisterType? firstRegisterType => RegisterType.Read;

        public override ControlBits controlBits => new(false, false, false, true, false, false, false);

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
