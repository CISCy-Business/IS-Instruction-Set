using InstructionSetProject.Backend.DynamicPipeline;

namespace InstructionSetProject.Backend
{
    public static class DynamicPipelineExecutor
    {
        public static DynamicPipelineExecution Execute(List<byte> machineCode) => Execute(GenerateInstructionList.FromBytes(machineCode));

        public static DynamicPipelineExecution Execute(string assemblyCode) => Execute(GenerateInstructionList.FromString(assemblyCode));

        public static DynamicPipelineExecution Execute(InstructionList instructions)
        {
            var exec = new DynamicPipelineExecution(instructions);
            return exec;
        }
    }
}
