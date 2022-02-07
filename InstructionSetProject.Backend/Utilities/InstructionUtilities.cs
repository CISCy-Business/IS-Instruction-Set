using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Utilities
{
    public static class InstructionUtilities
    {
        public static InstructionType GetInstructionType(List<byte> machineCode)
        {
            var firstByte = machineCode[0];

            if (firstByte >> 5 == 0)
                return InstructionType.R0;
            if (firstByte >> 5 == 1)
                return InstructionType.R1;
            if (firstByte >> 5 == 2)
                return InstructionType.R2;
            if (firstByte >> 5 == 3)
                return InstructionType.R3;
            if (firstByte >> 6 == 2)
                return InstructionType.Immediate;
            if (firstByte >> 6 == 3)
                return InstructionType.Jump;
            
            throw new Exception("Instruction does not match any instruction type pattern.");
        }
    }

    public enum InstructionType
    {
        R0,
        R1,
        R2,
        R3,
        Immediate,
        Jump
    }
}
