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

        public (ushort opcode, ushort? operand) Assemble()
        {
            return (GetOpCode(), null);
        }

        public string Disassemble()
        {
            return GetMnemonic();
        }

        public void ParseInstruction((ushort opcode, ushort? operand) machineCode)
        {
        }

        public void ParseInstruction(string assemblyCode)
        {
        }
    }
}
