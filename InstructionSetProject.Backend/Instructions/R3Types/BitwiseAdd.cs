using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R3Types
{
    public class BitwiseAdd : R3Instruction
    {
        public const string Mnemonic = "ADD";

        public const ushort OpCode = 0b0110_0000_0000_0000;

        public override string GetMnemonic()
        {
            return BitwiseAdd.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return BitwiseAdd.OpCode;
        }
    }
}
