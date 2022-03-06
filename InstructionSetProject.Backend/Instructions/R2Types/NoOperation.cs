using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.R2Formats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.R2Types
{
    public class NoOperation : R2NoOperands
    {
        public const string Mnemonic = "NOP";

        public const ushort OpCode = 0b0000_0000_0100_0000;

        public override ControlBits controlBits => new(false, false, false, false, false, false, false);

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
