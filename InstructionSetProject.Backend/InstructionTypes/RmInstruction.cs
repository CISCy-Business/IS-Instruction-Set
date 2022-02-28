using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public abstract class RmInstruction : IInstruction, IImmediateInstruction
    {
        public ushort AddressingModeOrRegister;
        public ushort DestinationRegister;
        public short Immediate;

        public ushort lengthInBytes => 4;

        public abstract FunctionBits functionBits { get; }

        public const ushort BitwiseMask = 0b1111_1111_1000_0000;

        public abstract string GetMnemonic();

        public abstract ushort GetOpCode();

        public abstract ushort AluOperation(ushort firstOperand, ushort secondOperand);

        public (ushort opcode, ushort? operand) Assemble()
        {
            var opcode = (ushort)(GetOpCode() | DestinationRegister | AddressingModeOrRegister);
            return (opcode, (ushort)Immediate);
        }

        public string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Registers.ParseIntDestination(DestinationRegister);
            assembly += ", ";
            if (AddressingModeOrRegister == 0b001_1000 || AddressingModeOrRegister == 0b010_0000)
            {
                assembly += Registers.ParseIntDestination((ushort)Immediate);
            }
            else
            {
                assembly += Immediate.ToString("X2");
            }
            assembly += ", ";
            assembly += AddressingMode.Get(AddressingModeOrRegister);

            return assembly;
        }

        public void ParseInstruction((ushort opcode, ushort? operand) machineCode)
        {
            AddressingModeOrRegister = (ushort)(machineCode.opcode & 0b111_1000);
            DestinationRegister = (ushort)(machineCode.opcode & 0b111);

            if (machineCode.operand == null)
                throw new ArgumentException("Operand to memory instruction cannot be null.");

            Immediate = (short)machineCode.operand;
        }

        public void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 4)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            DestinationRegister = Registers.ParseIntDestination(tokens[1].TrimEnd(','));

            AddressingModeOrRegister = AddressingMode.Get(tokens[3]);

            if (AddressingModeOrRegister == 0b001_1000 || AddressingModeOrRegister == 0b010_0000)
            {
                Immediate = (short)Registers.ParseIntDestination(tokens[2].TrimEnd(','));
            }
            else
            {
                Immediate = Convert.ToInt16(tokens[2].TrimEnd(','), 16);
            }
        }

        public ushort GenerateImmediate()
        {
            throw new NotImplementedException();
        }

        public bool CheckForLabel(string line)
        {
            var tokens = line.Split(' ');
            if (tokens.Length != 4)
                return false;
            var possibleLabel = tokens[2].Trim(',');
            var registerRegEx = new Regex("^[RrFf][0-7]$");
            if (registerRegEx.IsMatch(possibleLabel))
                return false;
            return !UInt16.TryParse(possibleLabel, out var result);
        }
    }
}
