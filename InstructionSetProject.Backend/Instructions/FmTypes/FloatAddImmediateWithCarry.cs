using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.FmFormats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.FmTypes
{
    public class FloatAddImmediateWithCarry : FmRegisterRegisterImmediate
    {
        public const string Mnemonic = "AIC";

        public const ushort OpCode = 0b1110_0100_1000_0000;

        public override ControlBits controlBits => new(true, true, false, false, false, false, true);

        public override AluOperation? aluOperation => AluOperation.FloatAddWithCarry;

        public override int cyclesNeededInExecute => 2;

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
