using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes.RmFormats
{
    public abstract class RmRegisterRegisterImmediate : RmInstruction, IImmediateInstruction
    {
        public override ushort? addressingMode { get => secondRegister; set { } }

        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Registers.ParseFirstInt(firstRegister ?? 0);
            assembly += ", ";
            assembly += Registers.ParseSecondInt(secondRegister ?? 0);
            assembly += ", ";
            assembly += (immediate ?? 0).ToString("X2");

            return assembly;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 4)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            firstRegister = Registers.ParseFirstInt(tokens[1].TrimEnd(','));

            secondRegister = Registers.ParseSecondInt(tokens[2].TrimEnd(','));

            immediate = Convert.ToUInt16(tokens[3], 16);
        }

        public ushort GenerateImmediate()
        {
            return immediate ?? 0;
        }
    }
}
