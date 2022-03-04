using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes.F2Formats
{
    public abstract class F2Register : F2Instruction
    {
        public override ushort? sourceRegister1 { get => null; set { } }

        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Registers.ParseFloatDestination(destinationRegister ?? 0);

            return assembly;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 2)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            destinationRegister = Registers.ParseFloatDestination(tokens[1]);
        }
    }
}
