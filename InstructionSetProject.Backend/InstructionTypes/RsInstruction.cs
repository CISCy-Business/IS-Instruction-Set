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
    public abstract class RsInstruction : IInstruction, IImmediateInstruction
    {
        public ushort DestinationRegister;
        public ushort SourceRegister;
        public ushort Immediate;

        public ushort lengthInBytes => 2;
        public abstract ControlBits controlBits { get; }

        public const ushort BitwiseMask = 0b1111_1100_0000_0000;

        public abstract string GetMnemonic();

        public abstract ushort GetOpCode();

        public abstract AluOperation? aluOperation { get; }

        public (ushort opcode, ushort? operand) Assemble()
        {
            var opcode = (ushort)(GetOpCode() | DestinationRegister | SourceRegister | Immediate);
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
            assembly += ", ";
            assembly += Immediate.ToString("X2");

            return assembly;
        }

        public void ParseInstruction((ushort opcode, ushort? operand) machineCode)
        {
            DestinationRegister = (ushort)(machineCode.opcode & 0b111);
            SourceRegister = (ushort)(machineCode.opcode & 0b11_1000);
            Immediate = (ushort) ((machineCode.opcode & 0b11_1100_0000) >> 6);
        }

        public void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 4)
                throw new Exception("Incorrect number fo tokens obtained from assembly instruction");

            DestinationRegister = Registers.ParseIntDestination(tokens[1].TrimEnd(','));

            SourceRegister = Registers.ParseIntFirstSource(tokens[2].TrimEnd(','));

            Immediate = Convert.ToUInt16(tokens[3], 16);
        }

        public ushort GenerateImmediate()
        {
            throw new NotImplementedException();
        }
    }
}
