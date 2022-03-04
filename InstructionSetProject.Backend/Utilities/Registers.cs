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

    public struct FlagsRegister
    {
        public bool Sign;
        public bool Parity;
        public bool Overflow;
        public bool Carry;
        public bool Zero;

        public ushort AsRegisterValue()
        {
            ushort flags = 0;
            if (Sign) flags += (ushort)Flags.Sign;
            if (Parity) flags += (ushort)Flags.Parity;
            if (Overflow) flags += (ushort)Flags.Overflow;
            if (Carry) flags += (ushort)Flags.Carry;
            if (Zero) flags += (ushort)Flags.Zero;
            return flags;
        }

        public bool IsFlagSet(Flags flag)
        {
            return ((ushort) flag & AsRegisterValue()) != 0;
        }
    }

    public enum Flags
    {
        Sign = 0b00001,
        Parity = 0b00010,
        Overflow = 0b00100,
        Carry = 0b010000,
        Zero = 0b10000
    }
}
