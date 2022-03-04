using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.R3Formats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.R3Types
{
    public class BitwiseOr : R3RegisterRegisterRegister
    {
        public const string Mnemonic = "ORR";

        public const ushort OpCode = 0b0100_1110_0000_0000;

        public override ControlBits controlBits => throw new NotImplementedException();

        public override AluOperation? aluOperation => null;

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
