using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public abstract class R1Instruction : IInstruction
    {
        public ushort DestinationRegister;

        public ushort lengthInBytes => 2;
        public abstract FunctionBits functionBits { get; }

        public const ushort BitwiseMask = 0b1111_1111_1111_1000;

        public abstract string GetMnemonic();

        public abstract ushort GetOpCode();

        public abstract ushort AluOperation(ushort firstOperand, ushort secondOperand);

        public (ushort opcode, ushort? operand) Assemble()
        {
            var opcode = (ushort)(GetOpCode() | DestinationRegister);
            return (opcode, null);
        }

        public string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Registers.ParseDestination(DestinationRegister);

            return assembly;
        }

        public void ParseInstruction((ushort opcode, ushort? operand) machineCode)
        {
            DestinationRegister = (ushort)(machineCode.opcode & 0b111);
        }

        public void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 2)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            DestinationRegister = Registers.ParseDestination(tokens[1]);
        }
    }
}
