using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.FmFormats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.FmTypes
{
    public class LoadFloat : FmRegisterImmediateAddrMode
    {
        public const string Mnemonic = "LDF";

        public const ushort OpCode = 0b1110_0000_0000_0000;

        public override ControlBits controlBits => new(true, (addressingMode != 0b001_0000 && addressingMode != 0b001_1000), true, false, true, false, false);

        public override AluOperation? aluOperation => (addressingMode == 0b001_0000 || addressingMode == 0b001_1000) ? AluOperation.PassFirstOperandThrough : AluOperation.PassSecondOperandThrough;

        public override int cyclesNeededInMemory => 3;

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
