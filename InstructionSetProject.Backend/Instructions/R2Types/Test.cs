using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.R2Formats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.R2Types
{
    public class Test : R2RegisterRegister
    {
        public const string Mnemonic = "TST";

        public const ushort OpCode = 0b0000_0100_1100_0000;

        public override ControlBits controlBits => new(false, false, false, false, false, false, true);

        public override AluOperation? aluOperation => AluOperation.BitwiseAnd;

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
