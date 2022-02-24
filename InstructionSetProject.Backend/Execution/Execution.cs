using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Execution
{
    public class Execution
    {
        public Register<ushort> R0 = new(0, false);
        public Register<ushort> R1 = new();
        public Register<ushort> R2 = new();
        public Register<ushort> R3 = new();
        public Register<ushort> R4 = new();
        public Register<ushort> R5 = new();
        public Register<ushort> R6 = new();
        public Register<ushort> R7 = new();
        public Register<uint> InstructionPointer = new();
        public Register<uint> StackPointer = new();
        public Register<ushort> Flags = new();

        public List<byte> Memory = new();
    }
}
