using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.F2Formats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.F2Types
{
    public class PopFloat : F2Register
    {
        public const string Mnemonic = "PPF";

        public const ushort OpCode = 0b0010_0001_1100_0000;

        public override ControlBits controlBits => new(true, false, true, false, true, false, false);

        public override AluOperation? aluOperation => AluOperation.NoOperation;

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
