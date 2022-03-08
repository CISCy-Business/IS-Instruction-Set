using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public abstract class R3Instruction : IInstruction
    {
        public ushort lengthInBytes => 2;
        public abstract ControlBits controlBits { get; }
        public const ushort BitwiseMask = 0b1111_1110_0000_0000;
        public abstract AluOperation? aluOperation { get; }
        public ushort? firstRegister { get; set; }
        public ushort? secondRegister { get; set; }
        public ushort? thirdRegister { get; set; }
        public ushort? addressingMode { get => null; set { } }
        public ushort? immediate { get => null; set { } }
        public RegisterType? firstRegisterType => RegisterType.Write;
        public RegisterType? secondRegisterType => RegisterType.Read;
        public RegisterType? thirdRegisterType => RegisterType.Read;
        public virtual int cyclesNeededInExecute => 1;
        public virtual int cyclesNeededInMemory => 1;

        public abstract string GetMnemonic();
        public abstract ushort GetOpCode();
        public abstract string Disassemble();
        public abstract void ParseInstruction(string assemblyCode);

        public (ushort opcode, ushort? operand) Assemble()
        {
            var opcode = (ushort)(GetOpCode() | (firstRegister ?? 0) | (secondRegister ?? 0) | (thirdRegister ?? 0));
            return (opcode, null);
        }

        public void ParseInstruction((ushort opcode, ushort? operand) machineCode)
        {
            firstRegister = (ushort)(machineCode.opcode & 0b111);
            secondRegister = (ushort)(machineCode.opcode & 0b11_1000);
            thirdRegister = (ushort)(machineCode.opcode & 0b1_1100_0000);
        }
    }
}
