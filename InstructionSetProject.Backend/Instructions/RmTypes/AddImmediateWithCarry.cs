using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.RmFormats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.RmTypes
{
    public class AddImmediateWithCarry : RmRegisterRegisterImmediate
    {
        public const string Mnemonic = "AIC";

        public const ushort OpCode = 0b1100_0100_1000_0000;

        public override ControlBits controlBits => new(true, true, false, false, false, false, true);

        public override AluOperation? aluOperation => AluOperation.AddWithCarry;

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
