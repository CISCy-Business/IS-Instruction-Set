using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Utilities
{
    public static class Registers
    {
        public static string ParseIntDestination(ushort value)
        {
            if (value > 7)
                throw new ArgumentException($"Invalid Register Value: '{value}'");

            return $"r{value}";
        }

        public static string ParseIntFirstSource(ushort value) => ParseIntDestination((ushort)(value >> 3));
        public static string ParseIntSecondSource(ushort value) => ParseIntDestination((ushort)(value >> 6));

        public static ushort ParseIntDestination(string registerName)
        {
            if (registerName == null || registerName.Length != 2 ||
                (registerName[0] != 'r' && registerName[0] != 'R') ||
                registerName[1] < '0' || registerName[1] > '7')
                throw new ArgumentException($"Invalid Register: '{registerName}'");

            return (ushort) (registerName[1] - '0');
        }

        public static ushort ParseIntFirstSource(string registerName) => (ushort) (ParseIntDestination(registerName) << 3);
        public static ushort ParseIntSecondSource(string registerName) => (ushort) (ParseIntDestination(registerName) << 6);

        public static string ParseFloatDestination(ushort value)
        {
            if (value > 7)
                throw new ArgumentException($"Invalid Register Value: '{value}'");

            return $"f{value}";
        }

        public static string ParseFloatFirstSource(ushort value) => ParseFloatDestination((ushort)(value >> 3));
        public static string ParseFloatSecondSource(ushort value) => ParseFloatDestination((ushort)(value >> 6));

        public static ushort ParseFloatDestination(string registerName)
        {
            if (registerName == null || registerName.Length != 2 ||
                (registerName[0] != 'f' && registerName[0] != 'F') ||
                registerName[1] < '0' || registerName[1] > '7')
                throw new ArgumentException($"Invalid Register: '{registerName}'");

            return (ushort)(registerName[1] - '0');
        }

        public static ushort ParseFloatFirstSource(string registerName) => (ushort)(ParseFloatDestination(registerName) << 3);
        public static ushort ParseFloatSecondSource(string registerName) => (ushort)(ParseFloatDestination(registerName) << 6);
    }
}
