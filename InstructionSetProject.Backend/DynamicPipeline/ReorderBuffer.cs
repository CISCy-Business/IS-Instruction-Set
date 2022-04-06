using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.Instructions.F2Types;
using InstructionSetProject.Backend.Instructions.FmTypes;
using InstructionSetProject.Backend.Instructions.R2Types;
using InstructionSetProject.Backend.Instructions.RmTypes;
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

            while (buffers.Count > 0 && nextInstructionIndex == buffers.Peek().Index)
            {
                var commitInstruction = buffers.Dequeue();

                if (commitInstruction.instruction.controlBits.RegWrite)
                {
                    var writeReg = GetWriteRegister(commitInstruction.instruction);
                    if (commitInstruction.result != null)
                        writeReg.value = (ushort) commitInstruction.result;
                }
                else if (commitInstruction.instruction.controlBits.MemWrite)
                {
                    PerformMemWrite(commitInstruction);
                }
                
                nextInstructionIndex++;
            }
        }

        private Register<ushort> GetWriteRegister(IInstruction instr)
        {
            if (instr.firstRegisterType != RegisterType.Write)
                throw new Exception("Attempted to write with a non-write instruction");
            var regIndex = instr.firstRegister ?? 0;

            if (instr is F2Instruction or F3Instruction or FmInstruction)
            {
                switch (regIndex)
                {
                    case 1: return dataStructures.F1;
                    case 2: return dataStructures.F2;
                    case 3: return dataStructures.F3;
                    case 4: return dataStructures.F4;
                    case 5: return dataStructures.F5;
                    case 6: return dataStructures.F6;
                    case 7: return dataStructures.F7;
                    case 8: return dataStructures.MemoryBasePointer;
                    default: return dataStructures.F0;
                }
            }
            else
            {
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

        private void PerformMemWrite(InstructionInFlight instr)
        {
            if (instr.instruction is StoreWord || instr.instruction is StoreFloat)
            {
                var writeTarget = (instr.instruction.addressingMode == 0b001_0000 || instr.instruction.addressingMode == 0b001_1000) ? instr.rhsValue : instr.instruction.immediate;
                if (instr.lhsValue == null || instr.instruction.addressingMode == null || writeTarget == null)
                    throw new Exception("Null write values");
                dataStructures.Memory.WriteUshort(writeTarget ?? 0, instr.lhsValue ?? 0, instr.instruction.addressingMode ?? 0);
                return;
            }

            if (instr.instruction is PushWord || instr.instruction is PushFloat)
            {
                dataStructures.Memory.StackPushWord(instr.lhsValue ?? 0);
                return;
            }

            throw new Exception("Unsupported write instruction");
        }
    }
}
