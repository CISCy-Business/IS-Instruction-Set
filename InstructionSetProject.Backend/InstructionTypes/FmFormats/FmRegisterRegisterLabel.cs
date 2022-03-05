using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes.FmFormats
{
    public abstract class FmRegisterRegisterLabel : FmInstruction, IImmediateInstruction, ILabelInstruction
    {
        public override ushort? addressingMode { get => secondRegister; set { } }
        public override RegisterType? firstRegisterType => RegisterType.Read;

        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Registers.ParseFirstFloat(firstRegister ?? 0);
            assembly += ", ";
            assembly += Registers.ParseSecondFloat(secondRegister ?? 0);
            assembly += ", ";
            assembly += (immediate ?? 0).ToString("X2");

            return assembly;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 4)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            firstRegister = Registers.ParseFirstFloat(tokens[1].TrimEnd(','));

            secondRegister = Registers.ParseSecondFloat(tokens[2].TrimEnd(','));

            immediate = Convert.ToUInt16(tokens[3], 16);
        }

        public ushort GenerateImmediate()
        {
            return immediate ?? 0;
        }

        public bool CheckForLabel(string line)
        {
            var tokens = line.Split(' ');
            if (tokens.Length != 4)
                return false;
            var possibleLabel = tokens[3];
            return !UInt16.TryParse(possibleLabel, out var result);
        }
    }
}
