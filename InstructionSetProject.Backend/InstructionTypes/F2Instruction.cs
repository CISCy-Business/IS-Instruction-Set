using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public abstract class F2Instruction : IInstruction, IFloatInstruction
    {
        public ushort lengthInBytes => 2;
        public abstract ControlBits controlBits { get; }
        public const ushort BitwiseMask = 0b1111_1111_1100_0000;
        public abstract AluOperation? aluOperation { get; }
        public ushort? firstRegister { get; set; }
        public virtual RegisterType? firstRegisterType => RegisterType.Write;
        public virtual ushort? secondRegister { get; set; }
        public virtual RegisterType? secondRegisterType => RegisterType.Read;
        public ushort? thirdRegister { get => null; set { } }
        public RegisterType? thirdRegisterType => null;
        public ushort? addressingMode { get => null; set { } }
        public ushort? immediate { get => null; set { } }

        public abstract string GetMnemonic();
        public abstract ushort GetOpCode();
        public abstract string Disassemble();
        public abstract void ParseInstruction(string assemblyCode);


        public (ushort opcode, ushort? operand) Assemble()
        {
            var opcode = (ushort)(GetOpCode() | (firstRegister ?? 0) | (secondRegister ?? 0));
            return (opcode, null);
        }

        public void ParseInstruction((ushort opcode, ushort? operand) machineCode)
        {
            firstRegister = (ushort)(machineCode.opcode & 0b111);
            secondRegister = (ushort)(machineCode.opcode & 0b11_1000);
        }
    }
}
