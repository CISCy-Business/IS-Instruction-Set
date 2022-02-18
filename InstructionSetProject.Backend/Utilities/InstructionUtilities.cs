using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Utilities
{
    public static class InstructionUtilities
    {
        public static InstructionType GetInstructionType(List<byte> machineCode)
        {
            var firstByte = machineCode[0];

            if (firstByte >> 5 == 0)
                return InstructionType.R0;
            if (firstByte >> 5 == 1)
                return InstructionType.R1;
            if (firstByte >> 5 == 2)
                return InstructionType.R2;
            if (firstByte >> 5 == 6)
                return InstructionType.R2I;
            if (firstByte >> 5 == 3)
                return InstructionType.R3;
            if (firstByte >> 5 == 4)
                return InstructionType.Memory;
            if (firstByte >> 5 == 5)
                return InstructionType.Jump;
            
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
    }

    public enum InstructionType
    {
        R0,
        R1,
        R2,
        R2I,
        R3,
        Memory,
        Jump
    }
}
