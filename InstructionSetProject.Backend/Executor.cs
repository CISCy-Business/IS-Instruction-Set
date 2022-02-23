using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend
{
    public static class Executor
    {
        public static void Execute(List<byte> machineCode) => Execute(GenerateInstructionList.FromBytes(machineCode));

        public static void Execute(string assemblyCode) => Execute(GenerateInstructionList.FromString(assemblyCode));

        public static void Execute(List<IInstruction> instructions)
        {

        }
    }
}
