using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Utilities
{
    public static class InstructionUtilities
    {
        public static InstructionType GetInstructionType(List<byte> machineCode)
        {
            var firstByte = machineCode[0];

            if (firstByte >> 5 == 0b000)
                return InstructionType.R2;
            if (firstByte >> 5 == 0b001)
                return InstructionType.F2;
            if (firstByte >> 5 == 0b010)
                return InstructionType.R3;
            if (firstByte >> 5 == 0b011)
                return InstructionType.F3;
            if (firstByte >> 5 == 0b100 || firstByte >> 5 == 0b101)
                return InstructionType.Rs;
            if (firstByte >> 5 == 0b110)
                return InstructionType.Rm;
            if (firstByte >> 5 == 0b111)
                return InstructionType.Fm;

            throw new Exception("Instruction does not match any instruction type pattern.");
        }

        public static List<byte> ConvertToByteArray(ushort value)
        {
            var byteArray = new List<byte>();

            byteArray.Add((byte)(value >> 8));
            byteArray.Add((byte)(value & 0b1111_1111));

            return byteArray;
        }

        public static List<byte> ConvertToByteArray(uint value)
        {
            var byteArray = new List<byte>();

            byteArray.Add((byte)(value >> 24));
            byteArray.Add((byte)((value >> 16) & 0b1111_1111));
            byteArray.Add((byte)((value >> 8) & 0b1111_1111));
            byteArray.Add((byte)(value & 0b1111_1111));

            return byteArray;
        }

        public static ushort ConvertToUshort(List<byte> bytes)
        {
            if (bytes.Count != 2)
                throw new Exception("Incorrect number of bytes for this instruction type");

            ushort fullInstr = (ushort)(bytes[0] << 8);
            fullInstr += bytes[1];

            return fullInstr;
        }

        public static uint ConvertToUint(List<byte> bytes)
        {
            if (bytes.Count != 4)
                throw new Exception("Incorrect number of bytes for this instruction type");

            uint fullInstr = (uint)(bytes[0] << 24);
            fullInstr += (uint)(bytes[1] << 16);
            fullInstr += (uint)(bytes[2] << 8);
            fullInstr += bytes[3];

            return fullInstr;
        }

        public static string GetMnemonic(string instruction)
        {
            var tokens = instruction.Split(' ');
            return tokens[0];
        }

        public static bool IsFloatInstruction(string instruction)
        {
            var tokens = instruction.Split(' ');
            tokens = tokens.Select(token => token.TrimEnd(',')).ToArray();

            if (tokens.Length <= 1) return false;

            if (tokens[1].StartsWith('f') || tokens[1].StartsWith('F')) return true;

            return false;
        }

        public static ushort GetOpCode(ushort instruction)
        {
            switch (instruction >> 13)
            {
                case 0b000:
                    return (ushort)(R2Instruction.BitwiseMask & instruction);
                case 0b001:
                    return (ushort)(F2Instruction.BitwiseMask & instruction);
                case 0b010:
                    return (ushort)(R3Instruction.BitwiseMask & instruction);
                case 0b011:
                    return (ushort)(F3Instruction.BitwiseMask & instruction);
                case 0b100:
                case 0b101:
                    return (ushort)(RsInstruction.BitwiseMask & instruction);
                case 0b110:
                    return (ushort)(RmInstruction.BitwiseMask & instruction);
                case 0b111:
                    return (ushort)(FmInstruction.BitwiseMask & instruction);
                default:
                    throw new Exception("Instruction does not match any instruction type pattern.");
            }
        }
    }

    public enum InstructionType
    {
        R2,
        F2,
        R3,
        F3,
        Rs,
        Rm,
        Fm
    }
}
