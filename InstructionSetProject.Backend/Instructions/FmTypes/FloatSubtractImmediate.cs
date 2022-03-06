using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.FmFormats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.FmTypes
{
    public class FloatSubtractImmediate : FmRegisterRegisterImmediate
    {
        public const string Mnemonic = "SBI";

        public const ushort OpCode = 0b1110_0100_0000_0000;

        public override ControlBits controlBits => new(true, true, false, false, false, false, true);

        public override AluOperation? aluOperation => AluOperation.FloatSubtract;

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
