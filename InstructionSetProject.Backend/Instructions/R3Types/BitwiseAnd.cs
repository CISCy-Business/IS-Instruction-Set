using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.R3Formats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.R3Types
{
    public class BitwiseAnd : R3RegisterRegisterRegister
    {
        public const string Mnemonic = "AND";

        public const ushort OpCode = 0b0100_1100_0000_0000;

        public override ControlBits controlBits => new(true, false, false, false, false, false, true);

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
