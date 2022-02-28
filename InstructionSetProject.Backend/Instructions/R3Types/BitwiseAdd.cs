using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.R3Types
{
    public class BitwiseAdd : R3Instruction
    {
        public const string Mnemonic = "ADD";

        public const ushort OpCode = 0b0110_0000_0000_0000;

        public override FunctionBits functionBits => new(true, false, false, false, false, false);

        public override string GetMnemonic()
        {
            return Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return OpCode;
        }

        public override ushort AluOperation(ushort firstOperand, ushort secondOperand)
        {
            return (ushort)(firstOperand + secondOperand);
        }
    }
}
