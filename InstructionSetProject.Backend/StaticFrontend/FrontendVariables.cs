namespace InstructionSetProject.Backend.StaticFrontend
{
    public static class FrontendVariables
    {
        public static string currentCodeDisassembler { get; set; } = "";
        public static string currentCodeAssembler { get; set; } = "";
        public static string currentMachineCodeExecutor { get; set; } = "";
        public static string currentAssemblyCodeExecutor { get; set; } = "";

        public static bool darkMode { get; set; } = false;

        public static bool darkModeIndex { get; set; } = false;
        public static bool darkModeIndexChanged { get; set; } = false;

        public static bool darkModeDisassembler { get; set; } = false;
        public static bool darkModeDisassemblerChanged { get; set; } = false;

        public static bool darkModeAssembler { get; set; } = false;
        public static bool darkModeAssemblerChanged { get; set; } = false;

        public static bool darkModeExecutor { get; set; } = false;
        public static bool darkModeExecutorChanged { get; set; } = false;

        public static bool darkModeHelpPage { get; set; } = false;
        public static bool darkModeHelpChanged { get; set; } = false;

        public static bool darkModeSample { get; set; } = false;
        public static bool darkModeSampleChanged { get; set; } = false;

    }
}
