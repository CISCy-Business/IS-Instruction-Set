using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public abstract class R1Instruction : IInstruction
    {
        public ushort DestinationRegister;

        public const ushort BitwiseMask = 0b1111_1111_1111_1000;

        public abstract string GetMnemonic();

        public abstract ushort GetOpCode();

        public List<byte> Assemble()
        {
            var fullInstr = GetOpCode();
            fullInstr += DestinationRegister;

            return InstructionUtilities.ConvertToByteArray(fullInstr);
        }

        public string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Register.ParseDestination(DestinationRegister);

            return assembly;
        }

        public void ParseInstruction(List<byte> machineCode)
        {
            var fullInstr = InstructionUtilities.ConvertToUshort(machineCode);

            DestinationRegister = (ushort)(fullInstr & 0b111);
        }

        public void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 2)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            DestinationRegister = Register.ParseDestination(tokens[1]);
        }
    }
}
