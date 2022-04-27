namespace InstructionSetProject.Backend.Execution;

public class Cache : IMemoryLevel
{
    private CacheConfiguration _config;
    private IMemoryLevel _nextMemoryLevel { get; private set; }
    public List<CacheSet> Sets { get; private set; }

    public Cache(CacheConfiguration config, IMemoryLevel nextMemoryLevel)
    {
        _config = config;
        _nextMemoryLevel = nextMemoryLevel;

        Sets = new List<CacheSet>(_config.SetCount);

        for (int i = 0; i < _config.SetCount; i++)
            Sets.Add(new CacheSet(config));
    }
    
    public void WriteUshort(uint addr, ushort writeValue)
    {
        var set = GetCacheSet(addr);

        if (!set.IsDataPresent(addr, 2))
            LoadCacheLine(addr);

        set.WriteUshort(addr, writeValue);

        if (_config.WriteStrategy == CacheWriteStrategy.WriteThrough)
            _nextMemoryLevel.WriteUshort(addr, writeValue);
    }

    public void WriteByte(uint addr, byte writeValue)
    {
        var set = GetCacheSet(addr);
        if (!set.IsDataPresent(addr, 1))
            LoadCacheLine(addr);
        
        set.WriteByte(addr, writeValue);

        if (_config.WriteStrategy == CacheWriteStrategy.WriteThrough)
            _nextMemoryLevel.WriteByte(addr, writeValue);
    }

    public ushort ReadUshort(uint addr)
    {
        var set = GetCacheSet(addr);
        if (set.IsDataPresent(addr, 2))
        {
            return set.ReadUshort(addr);
        }

        LoadCacheLine(addr);
        
        return set.ReadUshort(addr);
    }

    public byte ReadByte(uint addr)
    {
        var set = GetCacheSet(addr);
        if (set.IsDataPresent(addr, 1))
        {
            return set.ReadByte(addr);
        }

        LoadCacheLine(addr);

        return set.ReadByte(addr);
    }

    public void WriteLine(uint addr, byte[] data)
    {
        var set = GetCacheSet(addr);
        set.WriteLine(addr, data);
    }

    public byte[] LoadCacheLine(uint address)
    {
        var set = GetCacheSet(address);
        if (set.IsDataPresent(address, 1))
        {
            return (byte[])set.GetLine(address).Clone();
        }
        
        var data = nextLevel.LoadCacheLine(address);
        if (!set.IsFull())
        {
            set.InsertLine(address, data);
            return (byte[])data.Clone();
        }

        var evictedLine = set.EvictLine();
        var evictedAddress = evictedLine.Address;
        var evictedData = (byte[])evictedLine.Data.Clone();
        set.InsertLine(address, data);
        
        if (Config.WriteStrategy == CacheWriteStrategy.WriteBack)
        {
            nextLevel.WriteLine((uint)evictedAddress, evictedData);
        }

        return (byte[])data.Clone();
    }

    public CacheSet GetCacheSet(uint address)
    {
        var offsetSize = Math.ILogB(Config.LineSize);
        var indexSize = Math.ILogB(Config.SetCount);
        var offsetMask = (1 << offsetSize) - 1;
        var indexMask = (1 << indexSize) - 1;
        var offset = address & offsetMask;
        var index = (address >> offsetSize) & indexMask;
        if (index < 0 || index > Config.SetCount)
            throw new Exception("Invalid cache set index!");

        return Sets[(int)index];
    }
}

public record CacheConfiguration(int Associativity = default, int LineSize = default, int SetCount = default, CacheEvictionStrategy EvictionStrategy = default, CacheWriteStrategy WriteStrategy = default) 
{
    public int LineCount => SetCount * Associativity;
}

public enum CacheEvictionStrategy
{
    FIFO,
    Random,
    LRU
}

public enum CacheWriteStrategy
{
    WriteBack,
    WriteThrough
}