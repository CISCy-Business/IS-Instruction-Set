using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions
{
    public class Add
    {
        public const byte OpCode = 0;
        public const string Label = "ADD";

        public static BitArray Assemble(string assemblyLine)
        {
            return new BitArray(1);
        }

        public static string Disassemble(IEnumerable<bool> machineLine)
        {
            return "";
        }
    }
}
