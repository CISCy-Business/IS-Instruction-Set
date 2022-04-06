using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend
{
    public static class StaticPipelineExecutor
    {
        public static StaticPipelineExecution Execute(List<byte> machineCode) => Execute(GenerateInstructionList.FromBytes(machineCode));

        public static StaticPipelineExecution Execute(string assemblyCode) => Execute(GenerateInstructionList.FromString(assemblyCode));

        public static StaticPipelineExecution Execute(InstructionList instructions)
        {
            var exec = new StaticPipelineExecution(instructions);
            return exec;
        }
    }
}
