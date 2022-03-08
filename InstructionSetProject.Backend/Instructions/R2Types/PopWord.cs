using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.R2Formats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.R2Types
{
    public class PopWord : R2Register
    {
        public const string Mnemonic = "PPW";

        public const ushort OpCode = 0b0000_0001_1100_0000;

        public override ControlBits controlBits => new(true, false, true, false, true, false, false);

        public override AluOperation? aluOperation => AluOperation.NoOperation;

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
