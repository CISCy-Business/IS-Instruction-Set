using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes.R3Formats
{
    public abstract class R3RegisterRegisterRegister : R3Instruction
    {
        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Registers.ParseIntDestination(destinationRegister ?? 0);
            assembly += ", ";
            assembly += Registers.ParseIntFirstSource(sourceRegister1 ?? 0);
            assembly += ", ";
            assembly += Registers.ParseIntSecondSource(sourceRegister2 ?? 0);

            return assembly;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 4)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            destinationRegister = Registers.ParseIntDestination(tokens[1].TrimEnd(','));

            sourceRegister1 = Registers.ParseIntFirstSource(tokens[2].TrimEnd(','));

            sourceRegister2 = Registers.ParseIntSecondSource(tokens[3]);
        }
    }
}
