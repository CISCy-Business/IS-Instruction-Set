using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public abstract class MemoryInstruction : IInstruction
    {
        public ushort AddressingMode;
        public ushort DestinationRegister;
        public short Immediate;

        public const ushort BitwiseMask = 0b1111_1111_1000_0000;

        public abstract string GetMnemonic();

        public abstract ushort GetOpCode();

        public (ushort opcode, ushort? operand) Assemble()
        {
            var opcode = (ushort)(GetOpCode() | DestinationRegister | AddressingMode);
            return (opcode, (ushort)Immediate);
        }

        public string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Register.ParseDestination(DestinationRegister);
            assembly += ", ";
            if (AddressingMode == 0b001_1000 || AddressingMode == 0b010_0000)
            {
                assembly += Register.ParseDestination((ushort)Immediate);
            }
            else
            {
                assembly += Immediate.ToString("X2");
            }
            assembly += ", ";
            assembly += Utilities.AddressingMode.Get(AddressingMode);

            return assembly;
        }

        public void ParseInstruction((ushort opcode, ushort? operand) machineCode)
        {
            AddressingMode = (ushort)(machineCode.opcode & 0b111_1000);
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

            DestinationRegister = Register.ParseDestination(tokens[1].TrimEnd(','));

            AddressingMode = Utilities.AddressingMode.Get(tokens[3]);

            if (AddressingMode == 0b001_1000 || AddressingMode == 0b010_0000)
            {
                Immediate = (short)Register.ParseDestination(tokens[2].TrimEnd(','));
            }
            else
            {
                Immediate = Convert.ToInt16(tokens[2].TrimEnd(','), 16);
            }
        }
    }
}
