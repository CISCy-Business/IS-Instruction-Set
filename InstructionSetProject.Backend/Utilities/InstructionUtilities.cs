using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Utilities
{
    public static class InstructionUtilities
    {
        public static InstructionType GetInstructionType(List<byte> machineCode)
        {
            int firstThreeBits = machineCode[0] >> 5;

            if (firstThreeBits == 5) // Both 4 and 5 are Rs types
                firstThreeBits = 4;

            return (InstructionType)firstThreeBits;
        }

        public static List<byte> ConvertToByteArray(ushort value)
        {
            return new List<byte>()
            {
                (byte)((value >> 8) & 0xFF), // First byte
                (byte)((value >> 0) & 0xFF), // Second byte
            };
        }

        public static List<byte> ConvertToByteArray(uint value)
        {
            return new List<byte>()
            {
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 08) & 0xFF),
                (byte)((value >> 00) & 0xFF),
            };
        }

        public static ushort ConvertToUshort(List<byte> bytes)
        {
            if (bytes.Count != 2)
                throw new Exception("Incorrect number of bytes for this instruction type");

            int fullInstr = 0;
            
            fullInstr += bytes[0] << 8;
            fullInstr += bytes[1] << 0;

            return (ushort)(fullInstr & 0xFFFF);
        }

        public static uint ConvertToUint(List<byte> bytes)
        {
            if (bytes.Count != 4)
                throw new Exception("Incorrect number of bytes for this instruction type");

            int fullInstr = 0;

            fullInstr += bytes[0] << 24;
            fullInstr += bytes[1] << 16;
            fullInstr += bytes[2] << 08;
            fullInstr += bytes[3] << 00;

            return (uint)fullInstr;
        }

        public static string GetMnemonic(string instruction)
        {
            var tokens = instruction.Split(' ');
            return tokens[0];
        }

        public static bool IsFloatInstruction(string instruction)
        {
            var tokens = instruction.Split(" ,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length <= 1) return false;

            // Check if first argument begins with an 'f'
            if (Char.ToLower(tokens[1][0]) == 'f')
                return true;

            return false;
        }

        public static ushort GetOpCode(ushort instruction)
        {
            int firstThreeBits = instruction >> 13;

            ushort bitwiseMask = (firstThreeBits) switch
            {
                0b000 => R2Instruction.BitwiseMask,
                0b001 => F2Instruction.BitwiseMask,
                0b010 => R3Instruction.BitwiseMask,
                0b011 => F3Instruction.BitwiseMask,
                0b100 => RsInstruction.BitwiseMask,
                0b101 => RsInstruction.BitwiseMask,
                0b110 => RmInstruction.BitwiseMask,
                0b111 => FmInstruction.BitwiseMask,
                _ => throw new Exception("Instruction does not match any instruction type pattern.")
            };

            return (ushort)(instruction & bitwiseMask);
        }
    }

    public enum InstructionType
    {
        R2, F2, R3, F3, Rs, Rm, Fm
    }
}
