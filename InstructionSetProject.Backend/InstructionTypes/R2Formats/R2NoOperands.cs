using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes.R2Formats
{
    public abstract class R2NoOperands : R2Instruction
    {
        public override ushort? firstRegister { get => null; set { } }
        public override ushort? secondRegister { get => null; set { } }
        public override RegisterType? firstRegisterType => null;
        public override RegisterType? secondRegisterType => null;

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
