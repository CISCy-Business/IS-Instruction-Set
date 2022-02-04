using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Instructions;

namespace InstructionSetProject.Backend
{
    public static class Assembler
    {
        public static BitArray Assemble(string assemblyCode)
        {
            var assemblyLines = assemblyCode.Split("\n");
            var machineCode = new BitArray(0);
            foreach (var line in assemblyLines)
            {
                var machineLine = ConvertLineToMachineCode(line);
                machineCode = BitArrayTools.Append(machineCode, machineLine);
            }
            return machineCode;
        }

        public static BitArray ConvertLineToMachineCode(string instructionLine)
        {
            var instructionName = instructionLine.Substring(0, instructionLine.IndexOf(' '));

            switch (instructionName)
            {
                case Add.Label:
                    return Add.Assemble(instructionLine);
                default:
                    throw new Exception("Unable to parse instruction line.");
            }
        }
    }
}
