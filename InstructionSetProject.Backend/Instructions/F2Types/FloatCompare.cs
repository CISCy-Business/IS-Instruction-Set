using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.F2Formats;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.Instructions.F2Types
{
    public class FloatCompare : F2RegisterRegister
    {
        public const string Mnemonic = "CMP";

        public const ushort OpCode = 0b0010_0100_1000_0000;

        public override RegisterType? firstRegisterType => RegisterType.Read;

        public override ControlBits controlBits => new (false, false, false, false, false, false, true);

        public override AluOperation? aluOperation => AluOperation.Subtract;

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
