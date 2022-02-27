using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.InstructionTypes
{
    internal interface IImmediateInstruction
    {
        ushort GenerateImmediate();
    }
}
