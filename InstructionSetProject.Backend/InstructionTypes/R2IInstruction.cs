using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public abstract class R2IInstruction : IInstruction
    {
        public ushort DestinationRegister;
        public ushort SourceRegister;
        public short Immediate;

        public ushort lengthInBytes => 4;
        public abstract FunctionBits functionBits { get; }

        public const ushort BitwiseMask = 0b1111_1111_1100_0000;

        public abstract string GetMnemonic();

        public abstract ushort GetOpCode();

        public abstract ushort AluOperation(ushort firstOperand, ushort secondOperand);

        public (ushort opcode, ushort? operand) Assemble()
        {
            var opcode = (ushort)(GetOpCode() | DestinationRegister | SourceRegister);
            return (opcode, (ushort)Immediate);
        }

        public virtual string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Registers.ParseDestination(DestinationRegister);
            assembly += ", ";
            assembly += Registers.ParseFirstSource(SourceRegister);
            assembly += ", ";
            assembly += Immediate.ToString("X2");

            return assembly;
        }

        public void ParseInstruction((ushort opcode, ushort? operand) machineCode)
        {
            DestinationRegister = (ushort) (machineCode.opcode & 0b111);
            SourceRegister = (ushort) (machineCode.opcode & 0b11_1000);

            if (machineCode.operand == null)
                throw new ArgumentException("Operand to R2 Instruction cannot be null.");

            Immediate = (short) machineCode.operand;
        }

        public virtual void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 4)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            DestinationRegister = Registers.ParseDestination(tokens[1].TrimEnd(','));

            SourceRegister = Registers.ParseFirstSource(tokens[2].TrimEnd(','));

            Immediate = Convert.ToInt16(tokens[3], 16);
        }
    }
}
