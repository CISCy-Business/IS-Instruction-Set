using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.StaticFrontend
{
    public static class FrontendVariables
    {
        public static string currentCodeDisassembler = "";
        public static string currentCodeAssembler = "";


        public static string getCurrentCodeDisassembler()
        {
            return currentCodeDisassembler;
        }

        public static void setCurrentCodeDisassembler(string currentCodeDis)
        {
            currentCodeDisassembler = currentCodeDis;
        }

        public static string getCurrentCodeAssembler()
        {
            return currentCodeAssembler;
        }

        public static void setCurrentCodeAssembler(string currentCodeAssem)
        {
            currentCodeAssembler = currentCodeAssem;
        }
    }
}
