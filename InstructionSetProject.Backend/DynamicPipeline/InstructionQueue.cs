using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.Instructions.RmTypes;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.DynamicPipeline
{
    public class InstructionQueue
    {
        public InstructionList instructions { get; set; }
        private Register<ushort> instructionPointer { get; set; }
        public List<InstructionInFlight> nextBatch { get; set; } = new();
        public int instructionIndex { get; set; } = 0;
        private DependencyManager depManager { get; set; }

        public InstructionQueue(InstructionList instructions, Register<ushort> instructionPointer, DependencyManager depManager)
        {
            this.instructions = instructions;
            this.instructionPointer = instructionPointer;
            this.depManager = depManager;
            LoadNextBatch();
        }

        public void LoadNextBatch()
        {
            while (nextBatch.Count < 5)
            {
                var nextInstr = GetNextInstruction();
                if (nextInstr == null) break;
                nextBatch.Add(nextInstr);
            }
        }

        private InstructionInFlight? GetNextInstruction()
        {
            if (instructionPointer.value > instructions.MaxOffset) return null;
            var instr = instructions.GetFromOffset(instructionPointer.value);
            if (instr == null) return null;
            if (instr is JumpUnconditional)
            {
                instructionPointer.value = instr.immediate ?? (ushort)(instructionPointer.value + instr.lengthInBytes);
                return GetNextInstruction();
            }

            instructionPointer.value += instr.lengthInBytes;

            var instrInFlight = new InstructionInFlight(instr, instructionIndex);
            depManager.CheckDependencies(instrInFlight);
            instructionIndex++;
            return instrInFlight;
        }
    }
}
