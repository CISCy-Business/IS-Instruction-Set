namespace InstructionSetProject.Backend.Execution
{
    public class Memory
    {
        public byte[] Bytes = new byte[1048576];
        private readonly Register<ushort> _stackPointer;
        private readonly Register<ushort> _memoryBasePointer;
        private readonly Register<ushort> _r7;

        private const uint TwentyBitMask = 0b1111_1111_1111_1111_1111;
        private const byte EightBitMask = 0b1111_1111;

        public Memory(Register<ushort> stackPointer, Register<ushort> memoryBasePointer, Register<ushort> r7)
        {
            _stackPointer = stackPointer;
            _memoryBasePointer = memoryBasePointer;
            _r7 = r7;
        }

        public void WriteUshort(ushort inputValue, ushort writeValue, ushort addrMode)
        {
            var addr = GetAddress(inputValue, addrMode);
            Bytes[addr] = (byte) (writeValue >> 8);
            Bytes[addr + 1] = (byte) (writeValue & EightBitMask);
        }

        public void WriteByte(ushort inputValue, byte writeValue, ushort addrMode)
        {
            var addr = GetAddress(inputValue, addrMode);
            Bytes[addr] = writeValue;
        }

        public ushort ReadUshort(ushort inputValue, ushort addrMode)
        {
            var addr = GetAddress(inputValue, addrMode);
            var value = (ushort)(Bytes[addr] << 8);
            value += Bytes[addr + 1];

            return value;
        }

        public byte ReadByte(ushort inputValue, ushort addrMode)
        {
            var addr = GetAddress(inputValue, addrMode);
            return Bytes[addr];
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

        private uint GetAddress(ushort inputValue, ushort addrMode)
        {
            switch (addrMode)
            {
                case 0b000_0000:
                case 0b001_0000:
                    return GetDirectAddress(inputValue);
                case 0b000_1000:
                case 0b001_1000:
                    return GetIndirectAddress(inputValue);
                case 0b010_0000:
                    return GetDirectAddress((ushort)(inputValue + _r7.value));
                case 0b010_1000:
                    return GetIndirectAddress((ushort)(inputValue + _r7.value));
                case 0b011_0000:
                    return GetOffsetAddress(_r7.value, inputValue);
                case 0b011_1000:
                    return GetOffsetAddress(inputValue, _r7.value);
                case 0b100_0000:
                    return GetDirectAddress((ushort)(inputValue + _stackPointer.value));
                case 0b100_1000:
                    return GetIndirectAddress((ushort)(inputValue + _stackPointer.value));
                case 0b101_0000:
                    return GetOffsetAddress(_stackPointer.value, inputValue);
                case 0b101_1000:
                    return GetOffsetAddress(inputValue, _stackPointer.value);
                case 0b110_0000:
                    return GetDirectAddress((ushort)(inputValue + _r7.value + _stackPointer.value));
                case 0b110_1000:
                    return GetIndirectAddress((ushort)(inputValue + _r7.value + _stackPointer.value));
                case 0b111_0000:
                    return GetOffsetAddress((ushort)(_stackPointer.value + _r7.value), inputValue);
                case 0b111_1000:
                    return GetOffsetAddress((ushort)(_stackPointer.value + inputValue), _r7.value);
                default:
                    throw new Exception("Invalid addressing type");
            }
        }

        private uint GetDirectAddress(ushort inputValue)
        {
            uint fullAddr = (uint)(_memoryBasePointer.value << 16);
            fullAddr += inputValue;
            return fullAddr & TwentyBitMask;
        }

        private uint GetIndirectAddress(ushort inputValue)
        {
            uint firstAddr = (uint) (_memoryBasePointer.value << 16);
            firstAddr += inputValue;
            
            var fullAddr = (uint) (Bytes[firstAddr & TwentyBitMask] << 24);
            fullAddr += (uint) (Bytes[(firstAddr + 1) & TwentyBitMask] << 16);
            fullAddr += (uint)(Bytes[(firstAddr + 2) & TwentyBitMask] << 8);
            fullAddr += Bytes[(firstAddr + 3) & TwentyBitMask];

            return fullAddr;
        }

        private uint GetOffsetAddress(ushort firstMemAddr, ushort secondMemOffset)
        {
            uint firstAddr = (uint) (_memoryBasePointer.value << 16);
            firstAddr += firstMemAddr;

            var fullAddr = (uint)(Bytes[firstAddr & TwentyBitMask] << 24);
            fullAddr += (uint)(Bytes[(firstAddr + 1) & TwentyBitMask] << 16);
            fullAddr += (uint)(Bytes[(firstAddr + 2) & TwentyBitMask] << 8);
            fullAddr += Bytes[(firstAddr + 3) & TwentyBitMask];

            return fullAddr + secondMemOffset;
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
