using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.RmFormats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.RmTypes
{
    public class AndImmediate : RmRegisterRegisterImmediate
    {
        public const string Mnemonic = "ANI";

        public const ushort OpCode = 0b1100_0101_1000_0000;

        public override ControlBits controlBits => new (true, true, false, false, false, false, true);

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
