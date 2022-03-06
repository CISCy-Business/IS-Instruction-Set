using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.R2Formats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.R2Types
{
    public class BitwiseNot : R2RegisterRegister
    {
        public const string Mnemonic = "NOT";

        public const ushort OpCode = 0b0000_0011_1000_0000;

        public override ControlBits controlBits => new(true, false, false, false, false, false, true);

        public override AluOperation? aluOperation => AluOperation.BitwiseNot;

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
