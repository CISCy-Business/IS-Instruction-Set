using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.Execution
{
    public class PipelineDataStructures
    {
        public Register<ushort> R0 = new("R0",0, false);
        public Register<ushort> R1 = new("R1");
        public Register<ushort> R2 = new("R2");
        public Register<ushort> R3 = new("R3");
        public Register<ushort> R4 = new("R4");
        public Register<ushort> R5 = new("R5");
        public Register<ushort> R6 = new("R6");
        public Register<ushort> R7 = new("R7");

        public Register<ushort> F0 = new("F0", 0, false);
        public Register<ushort> F1 = new("F1");
        public Register<ushort> F2 = new("F2");
        public Register<ushort> F3 = new("F3");
        public Register<ushort> F4 = new("F4");
        public Register<ushort> F5 = new("F5");
        public Register<ushort> F6 = new("F6");
        public Register<ushort> F7 = new("F7");

        public Register<ushort> InstructionPointer = new("IP");
        public Register<ushort> StackPointer = new("SP", 0xFFFF);
        public Register<ushort> MemoryBasePointer = new("MP");
        public FlagsRegister Flags = new();

        public Memory Memory;

        public PipelineDataStructures()
        {
            Memory = new(StackPointer, MemoryBasePointer, R7);
        }
    }
}
