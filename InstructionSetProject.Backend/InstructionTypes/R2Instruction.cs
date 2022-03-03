using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public abstract class R2Instruction : IInstruction
    {
        public ushort DestinationRegister;
        public ushort SourceRegister;

        public ushort lengthInBytes => 2;
        public abstract ControlBits controlBits { get; }

        public const ushort BitwiseMask = 0b1111_1111_1100_0000;

        public abstract string GetMnemonic();

        public abstract ushort GetOpCode();

        public abstract AluOperation? aluOperation { get; }

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
            assembly += Registers.ParseIntDestination(DestinationRegister);
            assembly += ", ";
            assembly += Registers.ParseIntFirstSource(SourceRegister);

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

            DestinationRegister = Registers.ParseIntDestination(tokens[1].TrimEnd(','));

            SourceRegister = Registers.ParseIntFirstSource(tokens[2]);
        }
    }
}
