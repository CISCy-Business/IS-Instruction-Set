namespace InstructionSetProject.Backend.Execution
{
    public class Memory: IMemoryLevel
    {
        public byte[] Bytes = new byte[1048576];
        private readonly Register<ushort> _stackPointer;
        private int cacheLineSize { get; }
        
        private const byte EightBitMask = 0b1111_1111;

        public Memory(Register<ushort> stackPointer, int cacheLineSize)
        {
            _stackPointer = stackPointer;
            this.cacheLineSize = cacheLineSize;
        }

        public void WriteUshort(uint addr, ushort writeValue)
        {
            Bytes[addr] = (byte) (writeValue >> 8);
            Bytes[addr + 1] = (byte) (writeValue & EightBitMask);
        }

        public void WriteByte(uint addr, byte writeValue)
        {
            Bytes[addr] = writeValue;
        }

        public ushort ReadUshort(uint addr)
        {
            var value = (ushort)(Bytes[addr] << 8);
            value += Bytes[addr + 1];

            return value;
        }

        public byte ReadByte(uint addr)
        {
            return Bytes[addr];
        }

        public byte[] LoadCacheLine(uint address)
        {
            return Bytes.Skip((int)address).Take(cacheLineSize).ToArray();
        }

        public void WriteLine(uint address, byte[] data)
        {
            for (int i = 0; i < data.Length; i++, address++)
            {
                Bytes[address] = data[i];
            }
        }

        public void StackPushWord(ushort value)
        {
            var upperByte = (byte) (value >> 8);
            var lowerByte = (byte) (value & EightBitMask);

            Bytes[_stackPointer.value] = lowerByte;
            _stackPointer.value--;
            Bytes[_stackPointer.value] = upperByte;
            _stackPointer.value--;
        }

        public void StackPushByte(byte value)
        {
            Bytes[_stackPointer.value] = value;
            _stackPointer.value--;
        }

        public ushort StackPopWord()
        {
            _stackPointer.value++;
            var value = (ushort) (Bytes[_stackPointer.value] << 8);
            _stackPointer.value++;
            value += Bytes[_stackPointer.value];

            return value;
        }

        public byte StackPopByte()
        {
            _stackPointer.value++;
            return Bytes[_stackPointer.value];
        }

        public byte[] GetBytesAtAddress(uint address)
        {
            var arrayLength = 1000;
            byte[] result = new byte[arrayLength];
            Array.Copy(Bytes, address, result, 0, arrayLength);
            return result;
        }

        public void AddInstructionCode(List<byte> machineCode)
        {
            for (int i = 0; i < machineCode.Count; i++)
            {
                Bytes[i] = machineCode[i];
            }
        }
    }
}
