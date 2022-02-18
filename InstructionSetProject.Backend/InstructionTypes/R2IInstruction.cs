using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public abstract class R2IInstruction : IInstruction
    {
        public ushort DestinationRegister;
        public ushort SourceRegister;
        public short Immediate;

        public const ushort BitwiseMask = 0b1111_1111_1100_0000;

        public abstract string GetMnemonic();

        public abstract ushort GetOpCode();

        public List<byte> Assemble()
        {
            var firstHalfInstr = GetOpCode();
            firstHalfInstr += DestinationRegister;
            firstHalfInstr += SourceRegister;

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
            assembly += Register.ParseFirstSource(SourceRegister);
            assembly += ", ";
            assembly += Immediate.ToString("X2");

            return assembly;
        }

        public void ParseInstruction(List<byte> machineCode)
        {
            var fullInstr = InstructionUtilities.ConvertToUint(machineCode);
            var firstHalfInstr = (ushort) (fullInstr >> 16);

            DestinationRegister = (ushort) (firstHalfInstr & 0b111);

            SourceRegister = (ushort) (firstHalfInstr & 0b11_1000);

            Immediate = (short) (fullInstr & 0b1111_1111_1111_1111);
        }

        public void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 4)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            DestinationRegister = Register.ParseDestination(tokens[1].TrimEnd(','));

            SourceRegister = Register.ParseFirstSource(tokens[2].TrimEnd(','));

            Immediate = Convert.ToInt16(tokens[3], 16);
        }
    }
}
