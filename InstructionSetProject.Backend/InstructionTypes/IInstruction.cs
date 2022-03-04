using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public interface IInstruction
    {
        public ushort? destinationRegister { get; set; }
        public ushort? sourceRegister1 { get; set; }
        public ushort? sourceRegister2 { get; set; }
        public ushort? addressingMode { get; set; }
        public ushort? immediate { get; set; }
        public ControlBits controlBits { get; }
        public AluOperation? aluOperation { get; }
        public ushort lengthInBytes { get; }
        public string GetMnemonic();
        public ushort GetOpCode();
        public (ushort opcode, ushort? operand) Assemble();
        public string Disassemble();
        public void ParseInstruction(string assemblyCode);
        public void ParseInstruction((ushort opcode, ushort? operand) machineCode);
    }
}
