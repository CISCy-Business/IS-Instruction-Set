using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Execution
{
    public interface IExecution
    {
        void Step();
        void ClockTick();
        void Continue();
    }
}
