using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Instructions;

namespace InstructionSetProject.Backend
{
    internal static class GetInstruction
    {
        public static IInstruction FromOpCode(byte opCode)
        {
            switch (opCode)
            {
                case Add.OpCode:
                    return new Add();
                default:
                    throw new Exception($"Instruction not found with OpCode: {opCode}");
            }
        }

        public static IInstruction FromMnemonic(string mnemonic)
        {
            switch (mnemonic)
            {
                case Add.Mnemonic:
                    return new Add();
                default:
                    throw new Exception($"Instruction not found with Mnemonic: {mnemonic}");
            }
        }
    }
}
