using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions.Jump
{
    public class Call : IInstruction
    {
        public const string Mnemonic = "CAL";

        public const ushort OpCode = 0x1C0;

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
