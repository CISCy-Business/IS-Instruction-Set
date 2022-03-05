using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes.RmFormats
{
    public abstract class RmRegisterImmediateAddrMode : RmInstruction, IImmediateInstruction
    {
        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Registers.ParseFirstInt(firstRegister ?? 0);
            assembly += ", ";
            if (addressingMode == 0b001_0000 || addressingMode == 0b001_1000)
                assembly += Registers.ParseSecondInt(secondRegister ?? 0);
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

            firstRegister = Registers.ParseFirstInt(tokens[1].TrimEnd(','));

            addressingMode = AddressingMode.Get(tokens[3]);

            if (addressingMode == 0b001_0000 || addressingMode == 0b001_1000)
            {
                secondRegister = Registers.ParseFirstInt(tokens[2].TrimEnd(','));
                immediate = null;
            }
            else
            {
                immediate = Convert.ToUInt16(tokens[2].TrimEnd(','), 16);
                secondRegister = null;
            }
        }

        public ushort GenerateImmediate()
        {
            throw new NotImplementedException();
        }
    }
}
