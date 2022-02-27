using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend
{
    public static class StaticPipelineExecutor
    {
        public static IExecution Execute(List<byte> machineCode) => Execute(GenerateInstructionList.FromBytes(machineCode));

        public static IExecution Execute(string assemblyCode) => Execute(GenerateInstructionList.FromString(assemblyCode));

        public static IExecution Execute(InstructionList instructions)
        {
            var exec = new StaticPipelineExecution(instructions);
            return exec;
        }
    }
}
