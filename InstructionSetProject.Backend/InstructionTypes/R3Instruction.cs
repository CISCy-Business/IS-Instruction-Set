using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public abstract class R3Instruction : IInstruction
    {
        public ushort DestinationRegister;
        public ushort SourceRegister1;
        public ushort SourceRegister2;

        public const ushort BitwiseMask = 0b1111_1110_0000_0000;

        public abstract string GetMnemonic();

        public abstract ushort GetOpCode();
        
        public List<byte> Assemble()
        {
            var fullInstr = GetOpCode();
            fullInstr += DestinationRegister;
            fullInstr += SourceRegister1;
            fullInstr += SourceRegister2;

            return InstructionUtilities.ConvertToByteArray(fullInstr);
        }

        public string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Register.ParseDestination(DestinationRegister);
            assembly += ", ";
            assembly += Register.ParseFirstSource(SourceRegister1);
            assembly += ", ";
            assembly += Register.ParseSecondSource(SourceRegister2);

            return assembly;
        }

        public void ParseInstruction(List<byte> machineCode)
        {
            var fullInstr = InstructionUtilities.ConvertToUshort(machineCode);

            DestinationRegister = (ushort)(fullInstr & 0b111);

            SourceRegister1 = (ushort)(fullInstr & 0b11_1000);

            SourceRegister2 = (ushort)(fullInstr & 0b1_1100_0000);
        }

        public void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 4)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            DestinationRegister = Register.ParseDestination(tokens[1].TrimEnd(','));

            SourceRegister1 = Register.ParseFirstSource(tokens[2].TrimEnd(','));

            SourceRegister2 = Register.ParseSecondSource(tokens[3]);
        }
    }
}
