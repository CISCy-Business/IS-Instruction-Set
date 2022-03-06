using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.F3Formats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.F3Types
{
    public class FloatAdd : F3RegisterRegisterRegister
    {
        public const string Mnemonic = "ADD";

        public const ushort OpCode = 0b0110_0000_0000_0000;

        public override ControlBits controlBits => new(true, false, false, false, false, false, true);

        public override AluOperation? aluOperation => AluOperation.FloatAdd;

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
