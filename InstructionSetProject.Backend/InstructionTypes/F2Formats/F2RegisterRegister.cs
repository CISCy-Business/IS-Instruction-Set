using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes.F2Formats
{
    public abstract class F2RegisterRegister : F2Instruction
    {
        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Registers.ParseFloatDestination(destinationRegister ?? 0);
            assembly += ", ";
            assembly += Registers.ParseFloatFirstSource(sourceRegister1 ?? 0);

            return assembly;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 3)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            destinationRegister = Registers.ParseFloatDestination(tokens[1].TrimEnd(','));

            sourceRegister1 = Registers.ParseFloatFirstSource(tokens[2]);
        }
    }
}
