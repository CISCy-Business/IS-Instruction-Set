namespace InstructionSetProject.Backend.Execution;

public class CacheSet
{
    public int Associativity { get; }
    public int LineSize { get; }
    public CacheEvictionStrategy EvictionStrategy { get; }
    public CacheWriteStrategy WriteStrategy { get; }
    private int lruIndex { get; set; } = 0;

    // Int value is used for LRU and FIFO tracking
    public Dictionary<CacheLine, int> Lines { get; private set; }

    public CacheSet(int associativity, int lineSize, CacheEvictionStrategy evictionStrategy,
        CacheWriteStrategy writeStrategy)
    {
        Associativity = associativity;
        LineSize = lineSize;
        EvictionStrategy = evictionStrategy;
        WriteStrategy = writeStrategy;
        Lines = new Dictionary<CacheLine, int>(associativity);
        for (int i = 0; i < associativity; i++)
            Lines.Add(new CacheLine(LineSize), 0);
    }

    public bool IsFull()
    {
        foreach (var (line, position) in Lines)
        {
            if (line.IsValid == false)
                return false;
        }

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
        Lines[line] = lruIndex++;
    }

    public byte[] GetLine(uint address)
    {
        return (byte[])Lines.First(setIndex => setIndex.Key.Address == address).Key.Data.Clone();
    }

    public void WriteLine(uint address, byte[] data)
    {
        var line = Lines.First(setIndex => setIndex.Key.Address == address).Key;
        line.Data = data;
        if (EvictionStrategy == CacheEvictionStrategy.LRU)
        {
            Lines[line] = lruIndex++;
        }
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
        if (EvictionStrategy == CacheEvictionStrategy.LRU)
        {
            Lines[setValue.Key] = lruIndex++;
        }
    }

    public void WriteByte(uint address, byte writeValue)
    {
        var setValue = Lines.FirstOrDefault(setIndex => setIndex.Key.IsDataPresent(address, 1));
        if (setValue.Equals(default(KeyValuePair<CacheLine, int>)))
            throw new Exception("Cache line not found");
        
        setValue.Key.WriteByte(address, writeValue);
        if (EvictionStrategy == CacheEvictionStrategy.LRU)
        {
            Lines[setValue.Key] = lruIndex++;
        }
    }

    public ushort ReadUshort(uint address)
    {
        var setValue = Lines.FirstOrDefault(setIndex => setIndex.Key.IsDataPresent(address, 2));
        if (setValue.Equals(default(KeyValuePair<CacheLine, int>)))
            throw new Exception("Cache line not found");

        if (EvictionStrategy == CacheEvictionStrategy.LRU)
        {
            Lines[setValue.Key] = lruIndex++;
        }
        return setValue.Key.ReadUshort(address);
    }
    
    public byte ReadByte(uint address)
    {
        var setValue = Lines.FirstOrDefault(setIndex => setIndex.Key.IsDataPresent(address, 1));
        if (setValue.Equals(default(KeyValuePair<CacheLine, int>)))
            throw new Exception("Cache line not found");

        if (EvictionStrategy == CacheEvictionStrategy.LRU)
        {
            Lines[setValue.Key] = lruIndex++;
        }
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