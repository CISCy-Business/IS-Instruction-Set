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

        public List<byte> Assemble()
        {
            var firstHalfInstr = GetOpCode();
            firstHalfInstr += DestinationRegister;
            firstHalfInstr += AddressingMode;

            var fullInstr = (uint)(firstHalfInstr << 16);
            fullInstr += (uint)Immediate;

            return InstructionUtilities.ConvertToByteArray(fullInstr);
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

        public void ParseInstruction(List<byte> machineCode)
        {
            var fullInstr = InstructionUtilities.ConvertToUint(machineCode);
            var firstHalfInstr = (ushort)(fullInstr >> 16);

            AddressingMode = (ushort)(firstHalfInstr & 0b111_1000);

            DestinationRegister = (ushort)(firstHalfInstr & 0b111);

            Immediate = (short)(fullInstr & 0b1111_1111_1111_1111);
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
