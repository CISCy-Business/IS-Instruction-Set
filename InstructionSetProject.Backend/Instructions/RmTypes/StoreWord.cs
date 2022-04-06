using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.RmFormats;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.Instructions.RmTypes
{
    public class StoreWord : RmRegisterImmediateAddrMode
    {
        public const string Mnemonic = "STW";

        public const ushort OpCode = 0b1100_0001_1000_0000;

        public override RegisterType? firstRegisterType => RegisterType.Read;

        public override ControlBits controlBits => new(false, (addressingMode != 0b001_0000 && addressingMode != 0b001_1000), false, true, false, false, false);

        public override AluOperation? aluOperation => (addressingMode == 0b001_0000 || addressingMode == 0b001_1000) ? AluOperation.PassFirstOperandThrough : AluOperation.PassSecondOperandThrough;

        public override int cyclesNeededInMemory => 3;

        public override InstructionUnit instructionUnit => InstructionUnit.Memory;

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
