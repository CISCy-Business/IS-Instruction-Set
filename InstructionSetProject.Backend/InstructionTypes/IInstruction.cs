using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public interface IInstruction
    {
        public FunctionBits functionBits { get; }
        public ushort lengthInBytes { get; }
        public string GetMnemonic();
        public ushort GetOpCode();
        public (ushort opcode, ushort? operand) Assemble();
        public string Disassemble();
        public void ParseInstruction(string assemblyCode);
        public void ParseInstruction((ushort opcode, ushort? operand) machineCode);
        public ushort AluOperation(ushort firstOperand, ushort secondOperand);
    }
}
