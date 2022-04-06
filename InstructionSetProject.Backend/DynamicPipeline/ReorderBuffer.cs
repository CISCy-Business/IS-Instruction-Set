using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.DynamicPipeline
{
    public class ReorderBuffer
    {
        public PriorityQueue<InstructionInFlight, int> buffers { get; set; }
        public PipelineDataStructures dataStructures { get; set; }
        public int nextInstructionIndex { get; set; }

        public ReorderBuffer(PipelineDataStructures dataStructures)
        {
            this.dataStructures = dataStructures;
            buffers = new();
            nextInstructionIndex = 0;
        }

        public void ClockTick()
        {
            if (buffers.Count == 0) return;

            while (nextInstructionIndex == buffers.Peek().Index)
            {
                var commitInstruction = buffers.Dequeue();

                if (commitInstruction.instruction.controlBits.RegWrite)
                {
                    var writeReg = GetWriteRegister(commitInstruction.instruction);
                    if (commitInstruction.result != null)
                        writeReg.value = (ushort) commitInstruction.result;
                }
                
                nextInstructionIndex++;
            }
        }

        private Register<ushort> GetWriteRegister(IInstruction instr)
        {
            if (instr.firstRegisterType != RegisterType.Write)
                throw new Exception("Attempted to write with a non-write instruction");
            var regIndex = instr.firstRegister ?? 0;

            switch (regIndex)
            {
                case 1: return dataStructures.R1;
                case 2: return dataStructures.R2;
                case 3: return dataStructures.R3;
                case 4: return dataStructures.R4;
                case 5: return dataStructures.R5;
                case 6: return dataStructures.R6;
                case 7: return dataStructures.R7;
                case 8: return dataStructures.MemoryBasePointer;
                default: return dataStructures.R0;
            }
        }
    }
}
