using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.RmFormats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.RmTypes
{
    public class OrImmediate : RmRegisterRegisterImmediate
    {
        public const string Mnemonic = "ORI";

        public const ushort OpCode = 0b1100_0110_0000_0000;

        public override ControlBits controlBits => new(true, true, false, false, false, false, true);

        public override AluOperation? aluOperation => AluOperation.BitwiseOr;

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
