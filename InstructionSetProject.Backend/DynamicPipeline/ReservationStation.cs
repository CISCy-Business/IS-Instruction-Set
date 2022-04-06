using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.DynamicPipeline
{
    public class ReservationStation
    {
        public Queue<InstructionInFlight> instructions { get; set; }

        public ReservationStation()
        {
            instructions = new();
        }

        public InstructionInFlight? GetNextReadyInstruction()
        {
            if (instructions.Count == 0) return null;
            var nextInstr = instructions.Peek();
            if (nextInstr.StillHasDependencies())
                return null;
            return instructions.Dequeue();
        }
    }
}
