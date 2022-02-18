using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.MemoryTypes
{
    public class LoadWord : MemoryInstruction
    {
        public const string Mnemonic = "LDW";

        public const ushort OpCode = 0b1000_0000_0000_0000;

        public override string GetMnemonic()
        {
            return LoadWord.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return LoadWord.OpCode;
        }
    }
}
