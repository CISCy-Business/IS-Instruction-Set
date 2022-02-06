using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions.Stack
{
    public class PopWord : IInstruction
    {
        public const string Mnemonic = "POP";

        public const ushort OpCode = 0x204;

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
