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
            assembly += Registers.ParseFirstInt(firstRegister ?? 0);
            assembly += ", ";
            assembly += Registers.ParseSecondInt(secondRegister ?? 0);
            assembly += ", ";
            assembly += Registers.ParseThirdInt(thirdRegister ?? 0);

            return assembly;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 4)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            firstRegister = Registers.ParseFirstInt(tokens[1].TrimEnd(','));

            secondRegister = Registers.ParseSecondInt(tokens[2].TrimEnd(','));

            thirdRegister = Registers.ParseThirdInt(tokens[3]);
        }
    }
}
