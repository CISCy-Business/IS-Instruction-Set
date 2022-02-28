using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public abstract class F2Instruction : IInstruction
    {
        public ushort DestinationRegister;
        public ushort SourceRegister;

        public ushort lengthInBytes => 2;
        public abstract FunctionBits functionBits { get; }

        public const ushort BitwiseMask = 0b1111_1111_1100_0000;

        public abstract string GetMnemonic();

        public abstract ushort GetOpCode();

        public abstract ushort AluOperation(ushort firstOperand, ushort secondOperand);

        public (ushort opcode, ushort? operand) Assemble()
        {
            var opcode = (ushort)(GetOpCode() | DestinationRegister | SourceRegister);
            return (opcode, null);
        }

        public string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Registers.ParseFloatDestination(DestinationRegister);
            assembly += ", ";
            assembly += Registers.ParseFloatFirstSource(SourceRegister);

            return assembly;
        }

        public void ParseInstruction((ushort opcode, ushort? operand) machineCode)
        {
            DestinationRegister = (ushort)(machineCode.opcode & 0b111);
            SourceRegister = (ushort)(machineCode.opcode & 0b11_1000);
        }

        public void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 3)
                throw new Exception("Incorrect number fo tokens obtained from assembly instruction");

            DestinationRegister = Registers.ParseFloatDestination(tokens[1].TrimEnd(','));

            SourceRegister = Registers.ParseFloatFirstSource(tokens[2]);
        }
    }
}
