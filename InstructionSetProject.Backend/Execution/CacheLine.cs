namespace InstructionSetProject.Backend.Execution;

public class CacheLine
{
    private int size { get; }
    public bool IsValid { get; set; }
    public uint? Address { get; set; }
    public byte[] Data { get; set; }

    private uint? firstByte => Address;
    private uint? lastByte => Address != null ? (uint)(Address + size) : null;

    public CacheLine(int size)
    {
        this.size = size;
        IsValid = false;
        Address = null;
        Data = new byte[size];
    }

    public bool IsDataPresent(uint address, int bytesToRead)
    {
        for (int i = 0; i < bytesToRead; i++)
        {
            if (address < firstByte || address > lastByte)
                return false;
            address++;
        }

        return true;
    }

    public void WriteUshort(uint address, ushort writeValue)
    {
        if (!IsDataPresent(address, 2))
            throw new Exception("Data not found in cache line");
        
        var arrayOffset = address - Address ?? 0;
        Data[arrayOffset] = (byte) (writeValue >> 8);
        Data[arrayOffset + 1] = (byte) (writeValue & 0b1111_1111);
    }

    public void WriteByte(uint address, byte writeValue)
    {
        if (!IsDataPresent(address, 2))
            throw new Exception("Data not found in cache line");

        var arrayOffset = Address - address ?? 0;
        Data[arrayOffset] = writeValue;
    }

    public ushort ReadUshort(uint address)
    {
        if (!IsDataPresent(address, 2))
            throw new Exception("Data not found in cache line");

        var arrayOffset = address - Address ?? 0;
        var value = (ushort) (Data[arrayOffset] << 8);
        value += Data[arrayOffset + 1];

        return value;
    }

    public byte ReadByte(uint address)
    {
        if (!IsDataPresent(address, 1))
            throw new Exception("Data not found in cache line");

        var arrayOffset = address - Address ?? 0;
        return Data[arrayOffset];
    }
}