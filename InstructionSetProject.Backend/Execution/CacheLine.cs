
namespace InstructionSetProject.Backend.Execution;

public class CacheLine
{
    private byte[] _data;
    public bool IsValid { get; set; }
    public uint StartAddress { get; set; }

    public CacheLine(uint size)
    {
        IsValid = false;
        _data = new byte[size];
    }

    public void LoadData(uint address, byte[] data)
    {
        StartAddress = address;
        WriteAll(data);
        IsValid = true;
    }

    public byte[] ReadAll()
    {
        byte[] copy = new byte[_data.Length];
        Array.Copy(_data, copy, _data.Length);
        return copy;
    }

    public void WriteAll(byte[] newData)
    {
        Array.Copy(newData, _data, Math.Min(newData.Length, _data.Length));
    }

    public byte? ReadByte(uint address)
    {
        if (!IsValidAddress(address))
            return null;

        return _data[CalcIndexFromAddress(address)];
    }

    public void WriteByte(uint address, byte value)
    {
        if (IsValidAddress(address))
            _data[CalcIndexFromAddress(address)] = value;
    }

    public ushort? ReadUShort(uint address)
    {
        if (!IsValidAddress(address, 2))
            return null;

        uint index = CalcIndexFromAddress(address);
        ushort value = 0;

        value += (ushort)(_data[index + 0] << 8);
        value += (ushort)(_data[index + 1] << 0);

        return value;
    }

    public void WriteUShort(uint address, ushort value)
    {
        if (!IsValidAddress(address, 2))
            return;

        uint index = CalcIndexFromAddress(address);

        _data[index + 0] = (byte)(value << 8);
        _data[index + 1] = (byte)(value << 0);
    }

    public bool IsValidAddress(uint address, uint numBytesToRead = 1)
    {
        uint firstAddressAvailable = StartAddress;
        uint lastAddressAvailable = StartAddress + (uint)_data.Length - 1;

        uint firstAddressNeeded = address;
        uint lastAddressNeeded = address + numBytesToRead - 1;

        if (firstAddressNeeded < firstAddressAvailable || lastAddressNeeded > lastAddressAvailable)
            return false;

        return true;
    }

    private uint CalcIndexFromAddress(uint address)
    {
        return address - StartAddress;
    }
}