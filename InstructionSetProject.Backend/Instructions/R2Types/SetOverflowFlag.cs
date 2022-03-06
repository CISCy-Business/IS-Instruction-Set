using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.R2Formats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.R2Types
{
    public class SetOverflowFlag : R2NoOperands
    {
        public const string Mnemonic = "STO";

        public const ushort OpCode = 0b0000_0110_0000_0000;

        public override ControlBits controlBits => new(false, false, false, false, false, false, false);

        public override AluOperation? aluOperation => AluOperation.SetOverflowFlag;

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
