using InstructionSetProject.Backend.DynamicPipeline;
using InstructionSetProject.Backend.Execution;

namespace InstructionSetProject.Backend
{
    public static class DynamicPipelineExecutor
    {
        public static DynamicPipelineExecution Execute(List<byte> machineCode, CacheConfiguration l1Config, CacheConfiguration l2Config) => Execute(GenerateInstructionList.FromBytes(machineCode), l1Config, l2Config);

        public static DynamicPipelineExecution Execute(string assemblyCode, CacheConfiguration l1Config, CacheConfiguration l2Config) => Execute(GenerateInstructionList.FromString(assemblyCode), l1Config, l2Config);

        public static DynamicPipelineExecution Execute(InstructionList instructions, CacheConfiguration l1Config, CacheConfiguration l2Config)
        {
            var exec = new DynamicPipelineExecution(instructions, l1Config, l2Config);
            return exec;
        }
    }
}
