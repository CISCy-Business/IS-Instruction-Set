using InstructionSetProject.Backend.InstructionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Execution
{
    public class PipelineStatistics
    {
        public int FlushCount { get; set; } = 0;
        public int ClockTicks { get; set; } = 0;

        public int R2InstructionCount { get; set; } = 0;
        public int R3InstructionCount { get; set; } = 0;
        public int RmInstructionCount { get; set; } = 0;
        public int RsInstructionCount { get; set; } = 0;
        public int F2InstructionCount { get; set; } = 0;
        public int F3InstructionCount { get; set; } = 0;
        public int FmInstructionCount { get; set; } = 0;


        public void StatInstructionType (IInstruction instr)
        {
            if (instr is R2Instruction)
            {
                R2InstructionCount++;
            }
            if (instr is R3Instruction)
            {
                R3InstructionCount++;
            }
            if (instr is RmInstruction)
            {
                RmInstructionCount++;
            }
            if (instr is RsInstruction)
            {
                RsInstructionCount++;
            }
            if (instr is F2Instruction)
            {
                F2InstructionCount++;
            }
            if (instr is F3Instruction)
            {
                F3InstructionCount++;
            }
            if (instr is FmInstruction)
            {
                FmInstructionCount++;
            }
        }
    }
}
