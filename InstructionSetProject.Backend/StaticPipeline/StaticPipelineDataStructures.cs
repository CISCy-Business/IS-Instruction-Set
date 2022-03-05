using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.StaticPipeline
{
    public class StaticPipelineDataStructures
    {
        public Register<ushort> R0 = new(0, false);
        public Register<ushort> R1 = new();
        public Register<ushort> R2 = new();
        public Register<ushort> R3 = new();
        public Register<ushort> R4 = new();
        public Register<ushort> R5 = new();
        public Register<ushort> R6 = new();
        public Register<ushort> R7 = new();

        public Register<ushort> F0 = new(0, false);
        public Register<ushort> F1 = new();
        public Register<ushort> F2 = new();
        public Register<ushort> F3 = new();
        public Register<ushort> F4 = new();
        public Register<ushort> F5 = new();
        public Register<ushort> F6 = new();
        public Register<ushort> F7 = new();

        public Register<ushort> InstructionPointer = new();
        public Register<ushort> StackPointer = new();
        public FlagsRegister Flags = new();

        public Memory Memory = new();
    }
}
