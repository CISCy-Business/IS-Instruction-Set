using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Instructions;
using InstructionSetProject.Backend.Instructions.Arithmetic;

namespace InstructionSetProject.Backend
{
    internal static class GetInstruction
    {
        public static IInstruction FromOpCode(ushort opCode)
        {
            switch (opCode)
            {
                case BitwiseAdd.OpCode:
                    return new BitwiseAdd();
                default:
                    throw new Exception($"Instruction not found with OpCode: {opCode}");
            }
        }

        public static IInstruction FromMnemonic(string mnemonic)
        {
            switch (mnemonic)
            {
                case BitwiseAdd.Mnemonic:
                    return new BitwiseAdd();
                default:
                    throw new Exception($"Instruction not found with Mnemonic: {mnemonic}");
            }
        }
    }
}
