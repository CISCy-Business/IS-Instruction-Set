using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend
{
    public class InstructionList
    {
        public List<IInstruction> Instructions => InstructionOffsetDictionary.Values.ToList();
        public Dictionary<ushort, IInstruction> InstructionOffsetDictionary = new();
        public Dictionary<string, ushort> LabelOffsetDictionary = new();
        public int Count => InstructionOffsetDictionary.Count;
        public ushort MaxOffset => InstructionOffsetDictionary.Last().Key;

        private ushort _currentOffset = 0;

        public void AddInstruction(IInstruction instr)
        {
            InstructionOffsetDictionary.Add(_currentOffset, instr);
            _currentOffset += instr.lengthInBytes;
        }

        public void AddLabel(string label)
        {
            LabelOffsetDictionary.Add(label, _currentOffset);
        }

        public IInstruction GetFromIndex(int index)
        {
            return Instructions[index];
        }

        public IInstruction? GetFromOffset(int offset)
        {
            if (offset < 0)
                return null;
            return InstructionOffsetDictionary[(ushort)offset];
        }

        public ushort GetOffsetFromLabel(string label)
        {
            return LabelOffsetDictionary[label];
        }
    }
}
