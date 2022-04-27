namespace InstructionSetProject.Backend.Execution;

public interface IMemoryLevel
{
    public void WriteUshort(uint addr, ushort writeValue);
    public void WriteByte(uint addr, byte writeValue);
    public ushort ReadUshort(uint addr);
    public byte ReadByte(uint addr);
    public byte[] LoadCacheLine(uint address);
    public void WriteLine(uint address, byte[] data);
}