using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.StaticPipeline
{
    public class FunctionBits
    {
        public bool RegWrite;
        public bool ALUSrc;
        public bool MemRead;
        public bool MemWrite;
        public bool MemToReg;
        public bool PCSrc;

        public FunctionBits(bool regWrite, bool aluSrc, bool memRead, bool memWrite, bool memToReg, bool pcSrc)
        {
            RegWrite = regWrite;
            ALUSrc = aluSrc;
            MemRead = memRead;
            MemWrite = memWrite;
            MemToReg = memToReg;
            PCSrc = pcSrc;
        }
    }
}
