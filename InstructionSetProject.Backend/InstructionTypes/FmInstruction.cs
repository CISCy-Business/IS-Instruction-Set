using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

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
            var opcode = (ushort)(GetOpCode() | destinationRegister ?? 0 | addressingMode ?? 0);
            return (opcode, immediate);
        }

        public void ParseInstruction((ushort opcode, ushort? operand) machineCode)
        {
            addressingMode = (ushort)(machineCode.opcode & 0b111_1000);
            destinationRegister = (ushort)(machineCode.opcode & 0b111);
            sourceRegister1 = (ushort) (machineCode.opcode & 0b11_1000);

            if (machineCode.operand == null)
                throw new ArgumentException("Operand to memory instruction cannot be null.");

            immediate = machineCode.operand;
        }
    }
}
