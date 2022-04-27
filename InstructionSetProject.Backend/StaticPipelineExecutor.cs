using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend
{
    public static class StaticPipelineExecutor
    {
        public static StaticPipelineExecution Execute(List<byte> machineCode, CacheConfiguration l1Config, CacheConfiguration l2Config) => Execute(GenerateInstructionList.FromBytes(machineCode), l1Config, l2Config);

        public static StaticPipelineExecution Execute(string assemblyCode, CacheConfiguration l1Config, CacheConfiguration l2Config) => Execute(GenerateInstructionList.FromString(assemblyCode), l1Config, l2Config);

        public static StaticPipelineExecution Execute(InstructionList instructions, CacheConfiguration l1Config, CacheConfiguration l2Config)
        {
            var exec = new StaticPipelineExecution(instructions, l1Config, l2Config);
            return exec;
        }
    }
}
