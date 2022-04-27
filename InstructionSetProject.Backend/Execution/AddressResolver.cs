namespace InstructionSetProject.Backend.Execution;

public class AddressResolver
{
    private const uint TwentyBitMask = 0b1111_1111_1111_1111_1111;
    
    private Register<ushort> stackPointer { get; }
    private Register<ushort> memoryBasePointer { get; }
    private Register<ushort> r7 { get; }
    private Memory memory { get; }

    public AddressResolver(Register<ushort> stackPointer, Register<ushort> memoryBasePointer, Register<ushort> r7,
        Memory memory)
    {
        this.stackPointer = stackPointer;
        this.memoryBasePointer = memoryBasePointer;
        this.r7 = r7;
        this.memory = memory;
    }
    
    public uint GetAddress(ushort inputValue, ushort addrMode)
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
                    return GetDirectAddress((ushort)(inputValue + r7.value));
                case 0b010_1000:
                    return GetIndirectAddress((ushort)(inputValue + r7.value));
                case 0b011_0000:
                    return GetOffsetAddress(r7.value, inputValue);
                case 0b011_1000:
                    return GetOffsetAddress(inputValue, r7.value);
                case 0b100_0000:
                    return GetDirectAddress((ushort)(inputValue + stackPointer.value));
                case 0b100_1000:
                    return GetIndirectAddress((ushort)(inputValue + stackPointer.value));
                case 0b101_0000:
                    return GetOffsetAddress(stackPointer.value, inputValue);
                case 0b101_1000:
                    return GetOffsetAddress(inputValue, stackPointer.value);
                case 0b110_0000:
                    return GetDirectAddress((ushort)(inputValue + r7.value + stackPointer.value));
                case 0b110_1000:
                    return GetIndirectAddress((ushort)(inputValue + r7.value + stackPointer.value));
                case 0b111_0000:
                    return GetOffsetAddress((ushort)(stackPointer.value + r7.value), inputValue);
                case 0b111_1000:
                    return GetOffsetAddress((ushort)(stackPointer.value + inputValue), r7.value);
                default:
                    throw new Exception("Invalid addressing type");
            }
        }

        private uint GetDirectAddress(ushort inputValue)
        {
            uint fullAddr = (uint)(memoryBasePointer.value << 16);
            fullAddr += inputValue;
            return fullAddr & TwentyBitMask;
        }

        private uint GetIndirectAddress(ushort inputValue)
        {
            uint firstAddr = (uint) (memoryBasePointer.value << 16);
            firstAddr += inputValue;
            
            var fullAddr = (uint) (memory.Bytes[firstAddr & TwentyBitMask] << 24);
            fullAddr += (uint) (memory.Bytes[(firstAddr + 1) & TwentyBitMask] << 16);
            fullAddr += (uint)(memory.Bytes[(firstAddr + 2) & TwentyBitMask] << 8);
            fullAddr += memory.Bytes[(firstAddr + 3) & TwentyBitMask];

            return fullAddr;
        }

        private uint GetOffsetAddress(ushort firstMemAddr, ushort secondMemOffset)
        {
            uint firstAddr = (uint) (memoryBasePointer.value << 16);
            firstAddr += firstMemAddr;

            var fullAddr = (uint)(memory.Bytes[firstAddr & TwentyBitMask] << 24);
            fullAddr += (uint)(memory.Bytes[(firstAddr + 1) & TwentyBitMask] << 16);
            fullAddr += (uint)(memory.Bytes[(firstAddr + 2) & TwentyBitMask] << 8);
            fullAddr += memory.Bytes[(firstAddr + 3) & TwentyBitMask];

            return fullAddr + secondMemOffset;
        }
}