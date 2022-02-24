using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.StaticFrontend
{
    public static class FrontendVariables
    {
        public static string currentCodeDisassembler { get; set; } = "";
        public static string currentCodeAssembler { get; set; } = "";
        public static string currentMachineCodeExecuter { get; set; } = "";
        public static string currentAssemblyCodeExecuter { get; set; } = "";
        
    }
}
