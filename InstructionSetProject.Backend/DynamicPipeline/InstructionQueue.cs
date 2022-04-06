using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.DynamicPipeline
{
    public class InstructionQueue
    {
        public InstructionList instructions { get; set; }
        private Register<ushort> instructionPointer { get; set; }
        private int instructionIndex { get; set; } = 0;

        public InstructionQueue(InstructionList instructions, Register<ushort> instructionPointer)
        {
            this.instructions = instructions;
            this.instructionPointer = instructionPointer;
        }
    }
}
