using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Utilities
{
    public static class Registers
    {
        public static string ParseDestination(ushort value)
        {
            if (value > 7)
                throw new ArgumentException($"Invalid Register Value: '{value}'");

            return $"r{value}";
        }

        public static string ParseFirstSource(ushort value) => ParseDestination((ushort)(value >> 3));
        public static string ParseSecondSource(ushort value) => ParseDestination((ushort)(value >> 6));

        public static ushort ParseDestination(string registerName)
        {
            if (registerName == null || registerName.Length != 2 ||
                (registerName[0] != 'r' && registerName[0] != 'R') ||
                registerName[1] < '0' || registerName[1] > '7')
                throw new ArgumentException($"Invalid Register: '{registerName}'");

            return (ushort) (registerName[1] - '0');
        }

        public static ushort ParseFirstSource(string registerName) => (ushort) (ParseDestination(registerName) << 3);
        public static ushort ParseSecondSource(string registerName) => (ushort) (ParseDestination(registerName) << 6);
    }

    public enum IntRegister
    {
        R0 = 0,
        R1 = 1,
        R2 = 2,
        R3 = 3,
        R4 = 4,
        R5 = 5,
        R6 = 6,
        R7 = 7
    }
}
