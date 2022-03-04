namespace InstructionSetProject.Backend.InstructionTypes.RmFormats
{
    public abstract class RmLabel : RmInstruction, IImmediateInstruction, ILabelInstruction
    {
        public override ushort? addressingMode { get => null; set { } }
        public override ushort? sourceRegister1 { get => null; set { } }
        public override ushort? destinationRegister { get => null; set { } }

        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += (immediate ?? 0).ToString("X2");

            return assembly;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 2)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            immediate = Convert.ToUInt16(tokens[1], 16);
        }

        public ushort GenerateImmediate()
        {
            return immediate ?? 0;
        }

        public bool CheckForLabel(string line)
        {
            var tokens = line.Split(' ');
            if (tokens.Length != 2)
                return false;
            var possibleLabel = tokens[1];
            return !UInt16.TryParse(possibleLabel, out var result);
        }
    }
}
