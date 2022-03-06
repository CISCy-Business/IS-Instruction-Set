using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.FmFormats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.FmTypes
{
    public class FloatXorImmediate : FmRegisterRegisterImmediate
    {
        public const string Mnemonic = "XRI";

        public const ushort OpCode = 0b1110_0110_1000_0000;

        public override ControlBits controlBits => new(true, true, false, false, false, false, true);

        public override AluOperation? aluOperation => AluOperation.BitwiseXor;

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
