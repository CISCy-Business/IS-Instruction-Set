using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions.Arithmetic
{
    public class RotateLeftCarry : IInstruction
    {
        public const string Mnemonic = "RLC";

        public const ushort OpCode = 0x4B;

        public List<byte> Assemble(string assemblyLine)
        {
            throw new NotImplementedException();
        }

        public string Disassemble(List<byte> machineLine)
        {
            throw new NotImplementedException();
        }
    }
}
