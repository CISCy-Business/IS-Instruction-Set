using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public interface IInstruction
    {
        public string GetMnemonic();
        public ushort GetOpCode();
        public List<byte> Assemble();
        public string Disassemble();

        public string GetAddressingModeString();
    }
}
