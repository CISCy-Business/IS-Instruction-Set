using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public interface IFlagInstruction
    {
        public Flags flagToCheck { get; }
        public bool flagEnabled { get; }
    }
}
