using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Execution;

namespace InstructionSetProject.Backend.StaticPipeline
{
    public class StaticPipelineDataStructures
    {
        public Register<ushort> R0 = new(0, false);
        public Register<ushort> R1 = new(1);
        public Register<ushort> R2 = new(2);
        public Register<ushort> R3 = new(4);
        public Register<ushort> R4 = new(4);
        public Register<ushort> R5 = new(5);
        public Register<ushort> R6 = new(6);
        public Register<ushort> R7 = new(7);

        public Register<ushort> InstructionPointer = new();
        public Register<ushort> StackPointer = new();
        public Register<ushort> Flags = new();

        public Memory Memory = new();
    }
}
