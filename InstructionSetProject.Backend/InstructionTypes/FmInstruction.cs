using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public abstract class FmInstruction : IInstruction
    {
        public ushort lengthInBytes => 4;
        public abstract ControlBits controlBits { get; }
        public const ushort BitwiseMask = 0b1111_1111_1000_0000;
        public abstract AluOperation? aluOperation { get; }
        public ushort? destinationRegister { get; set; }
        public virtual ushort? sourceRegister1 { get; set; }
        public ushort? sourceRegister2 { get => null; set { } }
        public virtual ushort? addressingMode { get; set; }
        public ushort? immediate { get; set; }

        public abstract string GetMnemonic();
        public abstract ushort GetOpCode();
        public abstract string Disassemble();
        public abstract void ParseInstruction(string assemblyCode);

        public (ushort opcode, ushort? operand) Assemble()
        {
            var opcode = (ushort)(GetOpCode() | (destinationRegister ?? 0) | (addressingMode ?? 0));
            return (opcode, immediate);
        }

        public void ParseInstruction((ushort opcode, ushort? operand) machineCode)
        {
            addressingMode = (ushort)(machineCode.opcode & 0b111_1000);
            destinationRegister = (ushort)(machineCode.opcode & 0b111);

            if (machineCode.operand == null)
                throw new ArgumentException("Operand to memory instruction cannot be null.");

            if (addressingMode == 0b001_0000 || addressingMode == 0b001_1000)
                sourceRegister1 = machineCode.operand;

            immediate = machineCode.operand;
        }
    }
}
