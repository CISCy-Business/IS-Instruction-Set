using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes.F3Formats
{
    public abstract class F3RegisterRegisterRegister : F3Instruction
    {
        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Registers.ParseFloatDestination(destinationRegister ?? 0);
            assembly += ", ";
            assembly += Registers.ParseFloatFirstSource(sourceRegister1 ?? 0);
            assembly += ", ";
            assembly += Registers.ParseFloatSecondSource(sourceRegister2 ?? 0);

            return assembly;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 4)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            destinationRegister = Registers.ParseFloatDestination(tokens[1].TrimEnd(','));

            sourceRegister1 = Registers.ParseFloatFirstSource(tokens[2].TrimEnd(','));

            sourceRegister2 = Registers.ParseFloatSecondSource(tokens[3]);
        }
    }
}
