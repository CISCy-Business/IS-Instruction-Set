using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public interface IInstruction
    {
        public ushort? firstRegister { get; set; }
        public RegisterType? firstRegisterType { get; }
        public ushort? secondRegister { get; set; }
        public RegisterType? secondRegisterType { get; }
        public ushort? thirdRegister { get; set; }
        public RegisterType? thirdRegisterType { get; }
        public ushort? addressingMode { get; set; }
        public ushort? immediate { get; set; }
        public ControlBits controlBits { get; }
        public AluOperation? aluOperation { get; }
        public ushort lengthInBytes { get; }
        public int cyclesNeededInExecute { get; }
        public int cyclesNeededInMemory { get; }
        public string GetMnemonic();
        public ushort GetOpCode();
        public (ushort opcode, ushort? operand) Assemble();
        public string Disassemble();
        public void ParseInstruction(string assemblyCode);
        public void ParseInstruction((ushort opcode, ushort? operand) machineCode);
    }
}
