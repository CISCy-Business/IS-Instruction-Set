using InstructionSetProject.Backend.InstructionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Instructions.R3Types;

namespace InstructionSetProject.Backend.Utilities
{
    public class InstructionManager
    {
        public static InstructionManager Instance = new();

        private Dictionary<ushort, string> keyDictionary = new();
        private Dictionary<string, Func<IInstruction>> valueDictionary = new();

        public InstructionManager()
        {
            Add(BitwiseAdd.OpCode, BitwiseAdd.Mnemonic, () => new BitwiseAdd());
        }

        public void Add(ushort opcode, string mnemonic, Func<IInstruction> constructor)
        {
            keyDictionary[opcode] = mnemonic;
            valueDictionary[mnemonic] = constructor;
        }

        public IInstruction Get(ushort opcode)
        {
            var mnemonic = keyDictionary[opcode];
            var constructor = valueDictionary[mnemonic];
            return constructor();
        }

        public IInstruction Get(string mnemonic)
        {
            var constructor = valueDictionary[mnemonic];
            return constructor();
        }
    }
}
