using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public abstract class RmInstruction : IInstruction
    {
        public ushort lengthInBytes => 4;
        public abstract ControlBits controlBits { get; }
        public const ushort BitwiseMask = 0b1111_1111_1000_0000;
        public abstract AluOperation? aluOperation { get; }
        public virtual ushort? firstRegister { get; set; }
        public virtual ushort? secondRegister { get; set; }
        public ushort? thirdRegister { get => null; set { } }
        public virtual ushort? addressingMode { get; set; }
        public ushort? immediate { get; set; }
        public virtual RegisterType? firstRegisterType => RegisterType.Write;
        public virtual RegisterType? secondRegisterType => RegisterType.Read;
        public RegisterType? thirdRegisterType => null;

        public abstract string GetMnemonic();
        public abstract ushort GetOpCode();
        public abstract string Disassemble();
        public abstract void ParseInstruction(string assemblyCode);

        public virtual (ushort opcode, ushort? operand) Assemble()
        {
            var opcode = (ushort)(GetOpCode() | (firstRegister ?? 0) | (addressingMode ?? 0));
            return (opcode, immediate ?? 0);
        }

        public virtual void ParseInstruction((ushort opcode, ushort? operand) machineCode)
        {
            addressingMode = (ushort)(machineCode.opcode & 0b111_1000);
            secondRegister = (ushort)(machineCode.opcode & 0b11_1000);
            firstRegister = (ushort)(machineCode.opcode & 0b111);

            if (machineCode.operand == null)
                throw new ArgumentException("Operand to memory instruction cannot be null.");

            immediate = machineCode.operand;
        }
    }
}
