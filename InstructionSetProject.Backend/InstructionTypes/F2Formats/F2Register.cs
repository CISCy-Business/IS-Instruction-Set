using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes.F2Formats
{
    public abstract class F2Register : F2Instruction
    {
        public override ushort? secondRegister { get => null; set { } }
        public override RegisterType? secondRegisterType => null;

        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Registers.ParseFirstFloat(firstRegister ?? 0);

            return assembly;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 2)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            firstRegister = Registers.ParseFirstFloat(tokens[1]);
        }
    }
}
