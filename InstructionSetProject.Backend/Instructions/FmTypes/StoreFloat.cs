using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.FmFormats;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.Instructions.FmTypes
{
    public class StoreFloat : FmRegisterImmediateAddrMode
    {
        public const string Mnemonic = "STF";

        public const ushort OpCode = 0b1110_0001_1000_0000;

        public override RegisterType? firstRegisterType => RegisterType.Read;

        public override ControlBits controlBits => new(false, (addressingMode != 0b001_0000 && addressingMode != 0b001_1000), false, true, false, false, false);

        public override AluOperation? aluOperation => (addressingMode == 0b001_0000 || addressingMode == 0b001_1000) ? AluOperation.PassFirstOperandThrough : AluOperation.PassSecondOperandThrough;

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
