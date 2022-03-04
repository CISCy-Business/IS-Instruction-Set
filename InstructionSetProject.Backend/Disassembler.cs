namespace InstructionSetProject.Backend
{
    public static class Disassembler
    {
        public static int progCounter = 0;
        public static string programCounter = "";
        public static bool immOrJump = false;
        public static string addrMode = "";
        public static string assemblyCode = "";
        public static int totalInstructions { get; set; } = 0;
        public static int arithmeticInstructions { get; set; } = 0;
        public static int generalInstructions { get; set; } = 0;
        public static int jumpInstructions { get; set; } = 0;
        public static int stackInstructions { get; set; } = 0;
        public static int immediateAddrMode { get; set; } = 0;
        public static int directAddrMode { get; set; } = 0;
        public static int indirectAddrMode { get; set; } = 0;
        public static int registerDirectAddrMode { get; set; } = 0;
        public static int registerIndirectAddrMode { get; set; } = 0;
        public static int indexDirectAddrMode { get; set; } = 0;
        public static int indexIndirectAddrMode { get; set; } = 0;
        public static int indexOffsetAddrMode { get; set; } = 0;
        public static int indexDefferedAddrMode { get; set; } = 0;
        public static int stackDirectAddrMode { get; set; } = 0;
        public static int stackIndirectAddrMode { get; set; } = 0;
        public static int stackOffsetAddrMode { get; set; } = 0;
        public static int stackIndexDirectAddrMode { get; set; } = 0;
        public static int stackIndexIndirectAddrMode { get; set; } = 0;
        public static int stackIndexOffsetAddrMode { get; set; } = 0;
        public static int stackIndexDefferedAddrMode { get; set; } = 0;

        public static string Disassemble(List<byte> machineCode) =>
            Disassemble(GenerateInstructionList.FromBytes(machineCode));

        public static string Disassemble(InstructionList instructions)
        {
            var disassembly = "";

            foreach (var instr in instructions.Instructions)
            {
                disassembly += instr.Disassemble() + "\n";
            }

            return disassembly;
        }
    }
}
