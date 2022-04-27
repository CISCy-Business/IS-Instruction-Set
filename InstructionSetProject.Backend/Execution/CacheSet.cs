namespace InstructionSetProject.Backend.Execution;

public class CacheSet
{
    public uint Associativity { get; }
    public uint LineSize { get; }
    public CacheEvictionStrategy EvictionStrategy { get; }
    public CacheWriteStrategy WriteStrategy { get; }
    private int lruIndex { get; set; } = 0;

    // Int value is used for LRU and FIFO tracking
    public Dictionary<CacheLine, int> Lines { get; }

    public CacheSet(CacheConfiguration config)
    {
        Associativity = config.Associativity;
        LineSize = config.LineSize;
        EvictionStrategy = config.EvictionStrategy;
        WriteStrategy = config.WriteStrategy;

        Lines = new Dictionary<CacheLine, int>((int)Associativity);

        for (int i = 0; i < Associativity; i++)
            Lines.Add(new CacheLine(LineSize), 0);
    }

    public bool IsFull()
    {
        foreach ((CacheLine line, _) in Lines)
            if (line.IsValid == false)
                return false;

        return true;
    }

    public void InsertLine(uint address, byte[] data)
    {
        if (IsFull())
            throw new Exception("Cache set is full! Line must be evicted before another line can be written!");

        var line = Lines.First(setIndex => setIndex.Key.IsValid == false).Key;
        line.Address = address;
        line.Data = data;
        line.IsValid = true;
    }

    public byte[] GetLine(uint address)
    {
        return (byte[])Lines.First(setIndex => setIndex.Key.Address == address).Key.Data.Clone();
    }

    public void WriteLine(uint address, byte[] data)
    {
        var line = Lines.First(setIndex => setIndex.Key.Address == address).Key;
        line.Data = data;
    }

    public bool IsDataPresent(uint address, int bytesToRead)
    {
        foreach (var (line, indexValue) in Lines)
        {
            if (line.IsDataPresent(address, bytesToRead) && line.IsValid)
                return true;
        }

        return false;
    }

    public void WriteUshort(uint address, ushort writeValue)
    {
        var setValue = Lines.FirstOrDefault(setIndex => setIndex.Key.IsDataPresent(address, 2));
        if (setValue.Equals(default(KeyValuePair<CacheLine, int>)))
            throw new Exception("Cache line not found");
        
        setValue.Key.WriteUshort(address, writeValue);
    }

    public void WriteByte(uint address, byte writeValue)
    {
        var setValue = Lines.FirstOrDefault(setIndex => setIndex.Key.IsDataPresent(address, 1));
        if (setValue.Equals(default(KeyValuePair<CacheLine, int>)))
            throw new Exception("Cache line not found");
        
        setValue.Key.WriteByte(address, writeValue);
    }

    public ushort ReadUshort(uint address)
    {
        var setValue = Lines.FirstOrDefault(setIndex => setIndex.Key.IsDataPresent(address, 2));
        if (setValue.Equals(default(KeyValuePair<CacheLine, int>)))
            throw new Exception("Cache line not found");

        return setValue.Key.ReadUshort(address);
    }
    
    public byte ReadByte(uint address)
    {
        var setValue = Lines.FirstOrDefault(setIndex => setIndex.Key.IsDataPresent(address, 1));
        if (setValue.Equals(default(KeyValuePair<CacheLine, int>)))
            throw new Exception("Cache line not found");

        return setValue.Key.ReadByte(address);
    }

    public CacheLine EvictLine() => EvictionStrategy switch
        {
            CacheEvictionStrategy.LRU => EvictLineLRU(),
            CacheEvictionStrategy.FIFO => EvictLineFIFO(),
            CacheEvictionStrategy.Random => EvictLineRandom(),
            _ => throw new Exception("Invalid eviction strategy")
        };

    private CacheLine EvictLineLRU()
    {
        var line = Lines.MinBy(line => line.Value).Key;
        line.IsValid = false;
        return line;
    }

    private CacheLine EvictLineFIFO()
    {
        var line = Lines.MinBy(line => line.Value).Key;
        line.IsValid = false;
        return line;
    }

    private CacheLine EvictLineRandom()
    {
        var rand = new Random();
        var lineIndex = rand.Next(LineSize);
        var line = Lines.ToList()[lineIndex].Key;
        line.IsValid = false;
        return line;
    }
}