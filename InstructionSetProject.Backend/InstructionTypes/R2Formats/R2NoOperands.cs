using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.InstructionTypes.R2Formats
{
    public abstract class R2NoOperands : R2Instruction
    {
        public override ushort? destinationRegister { get => null; set {} }
        public override ushort? sourceRegister1 { get => null; set { } }

        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();

            return assembly;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 1)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");
        }
    }
}
