namespace InstructionSetProject.Backend.Utilities
{
    public static class Registers
    {
        public static string ParseFirstInt(ushort value)
        {
            if (value > 7)
                throw new ArgumentException($"Invalid Register Value: '{value}'");

            return $"r{value}";
        }

        public static string ParseSecondInt(ushort value) => ParseFirstInt((ushort)(value >> 3));
        public static string ParseThirdInt(ushort value) => ParseFirstInt((ushort)(value >> 6));

        public static ushort ParseFirstInt(string registerName)
        {
            if (registerName == null || registerName.Length != 2 ||
                (registerName[0] != 'r' && registerName[0] != 'R') ||
                registerName[1] < '0' || registerName[1] > '7')
                throw new ArgumentException($"Invalid Register: '{registerName}'");

            return (ushort)(registerName[1] - '0');
        }

        public static ushort ParseSecondInt(string registerName) => (ushort)(ParseFirstInt(registerName) << 3);
        public static ushort ParseThirdInt(string registerName) => (ushort)(ParseFirstInt(registerName) << 6);

        public static string ParseFirstFloat(ushort value)
        {
            if (value > 7)
                throw new ArgumentException($"Invalid Register Value: '{value}'");

            return $"f{value}";
        }

        public static string ParseSecondFloat(ushort value) => ParseFirstFloat((ushort)(value >> 3));
        public static string ParseThirdFloat(ushort value) => ParseFirstFloat((ushort)(value >> 6));

        public static ushort ParseFirstFloat(string registerName)
        {
            if (registerName == null || registerName.Length != 2 ||
                (registerName[0] != 'f' && registerName[0] != 'F') ||
                registerName[1] < '0' || registerName[1] > '7')
                throw new ArgumentException($"Invalid Register: '{registerName}'");

            return (ushort)(registerName[1] - '0');
        }

        public static ushort ParseSecondFloat(string registerName) => (ushort)(ParseFirstFloat(registerName) << 3);
        public static ushort ParseThirdFloat(string registerName) => (ushort)(ParseFirstFloat(registerName) << 6);
    }

    public enum RegisterType
    {
        Read,
        Write
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
            return ((ushort)flag & AsRegisterValue()) != 0;
        }
    }

    public enum Flags
    {
        Sign = 0b00001,
        Parity = 0b00010,
        Overflow = 0b00100,
        Carry = 0b01000,
        Zero = 0b10000
    }
}
