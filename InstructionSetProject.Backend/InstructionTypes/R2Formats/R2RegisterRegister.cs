using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes.R2Formats
{
    public abstract class R2RegisterRegister : R2Instruction
    {
        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Registers.ParseIntDestination(destinationRegister ?? 0);
            assembly += ", ";
            assembly += Registers.ParseIntFirstSource(sourceRegister1 ?? 0);

            return assembly;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 3)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            destinationRegister = Registers.ParseIntDestination(tokens[1].TrimEnd(','));

            sourceRegister1 = Registers.ParseIntFirstSource(tokens[2]);
        }
    }
}
