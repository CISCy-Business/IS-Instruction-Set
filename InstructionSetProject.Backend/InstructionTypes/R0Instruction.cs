using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public abstract class R0Instruction : IInstruction
    {
        public const ushort BitwiseMask = 0b1111_1111_1111_1000;
        public abstract string GetMnemonic();

        public abstract ushort GetOpCode();

        public List<byte> Assemble()
        {
            var fullInstr = GetOpCode();

            return InstructionUtilities.ConvertToByteArray(fullInstr);
        }

        public string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();

            return assembly;
        }

        public void ParseInstruction(List<byte> machineCode)
        {
        }

        public void ParseInstruction(string assemblyCode)
        {
        }
    }
}
