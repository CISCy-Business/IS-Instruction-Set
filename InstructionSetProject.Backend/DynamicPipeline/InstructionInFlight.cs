using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.DynamicPipeline
{
    public class InstructionInFlight
    {
        public int Index;
        public IInstruction instruction { get; set; }
        public int cyclesRemainingInExecute { get; set; }
        public int cyclesRemainingInMemory { get; set; }
        public InstructionState state { get; set; }
        public ushort? lhsValue { get; set; }
        public int? lhsDependency { get; set; }
        public ushort? rhsValue { get; set; }
        public int? rhsDependency { get; set; }
        public ushort? result { get; set; }

        public InstructionInFlight(IInstruction instruction, int index)
        {
            Index = index;
            this.instruction = instruction;
            cyclesRemainingInExecute = instruction.cyclesNeededInExecute;
            cyclesRemainingInMemory = instruction.cyclesNeededInMemory;
            state = InstructionState.InstructionQueue;
        }

        public bool StillHasDependencies()
        {
            return lhsDependency != null || rhsDependency != null;
        }

        public void SetRhsFromDependency(ushort value)
        {
            rhsValue = value;
            rhsDependency = null;
        }

        public void SetLhsFromDependency(ushort value)
        {
            lhsValue = value;
            lhsDependency = null;
        }
    }

    public enum InstructionState
    {
        InstructionQueue,
        ReservationStation,
        LoadBuffer,
        Executing,
        AccessingMemory
    }
}
