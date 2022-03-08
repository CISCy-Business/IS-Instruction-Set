using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes.FmFormats
{
    public abstract class FmRegisterImmediateAddrMode : FmInstruction, IImmediateInstruction, IImmediateRegister
    {
        public override RegisterType? secondRegisterType => null;
        public override ushort? secondRegister { get => null; set {} }

        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Registers.ParseFirstFloat(firstRegister ?? 0);
            assembly += ", ";
            if (addressingMode == 0b001_0000 || addressingMode == 0b001_1000)
                assembly += Registers.ParseFirstFloat(immediate ?? 0);
            else
                assembly += (immediate ?? 0).ToString("X2");
            assembly += ", ";
            assembly += AddressingMode.Get(addressingMode ?? 0);

            return assembly;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 4)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            firstRegister = Registers.ParseFirstFloat(tokens[1].TrimEnd(','));

            addressingMode = AddressingMode.Get(tokens[3]);

            if (addressingMode == 0b001_0000 || addressingMode == 0b001_1000)
            {
                immediate = Registers.ParseFirstFloat(tokens[2].TrimEnd(','));
            }
            else
            {
                immediate = Convert.ToUInt16(tokens[2].TrimEnd(','), 16);
            }
        }

        public ushort GenerateImmediate()
        {
            return immediate ?? 0;
        }
    }
}
