using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.Arithmetic
{
    public class BitwiseAdd : R3Instruction
    {
        public new const string Mnemonic = "ADD";

        public new const ushort OpCode = 0x31;

        public BitwiseAdd(R3Instruction instr)
        {
            base.OpCode = instr.OpCode;
            base.Mnemonic = instr.Mnemonic;
            DestinationRegister = instr.DestinationRegister;
            SourceRegister1 = instr.SourceRegister1;
            SourceRegister2 = instr.SourceRegister2;
        }

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
