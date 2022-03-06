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
            assembly += Registers.ParseFirstFloat(firstRegister ?? 0);
            assembly += ", ";
            assembly += Registers.ParseSecondFloat(secondRegister ?? 0);
            assembly += ", ";
            assembly += Registers.ParseThirdFloat(thirdRegister ?? 0);

            return assembly;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 4)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            firstRegister = Registers.ParseFirstFloat(tokens[1].TrimEnd(','));

            secondRegister = Registers.ParseSecondFloat(tokens[2].TrimEnd(','));

            thirdRegister = Registers.ParseThirdFloat(tokens[3]);
        }
    }
}
