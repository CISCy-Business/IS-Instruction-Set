using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions
{
    internal interface IInstruction
    {
        public List<byte> Assemble(string assemblyLine);
        public string Disassemble(List<byte> machineLine);
    }
}
