using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public  class R1Instruction : IInstruction
    {
        public string Mnemonic = "";
        public ushort OpCode;
        public bool HighLowBit;
        public byte DestinationRegister;

        public virtual string GetMnemonic()
        {
            return Mnemonic;
        }

        public virtual ushort GetOpCode()
        {
            return OpCode;
        }

        public virtual bool GetHighLowBit()
        {
            return HighLowBit;
        }

        public List<byte> Assemble()
        {
            var machineCode = new List<byte>();

            byte firstByte = 0;
            firstByte += (byte) (GetOpCode() >> 4);
            machineCode.Add(firstByte);

            byte secondByte = 0;
            secondByte += (byte) ((GetOpCode() & 0xF) << 4);
            if (GetHighLowBit()) secondByte += 8;
            secondByte += DestinationRegister;
            machineCode.Add(secondByte);

            return machineCode;
        }

        public string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += GetRegister.FromByte(DestinationRegister);

            return assembly;
        }

        public static R1Instruction ParseInstruction(List<byte> machineCode)
        {
            if (machineCode.Count != 2)
                throw new Exception("Incorrect number of bytes for this instruction type");

            var instr = new R1Instruction();

            instr.OpCode = (ushort)((machineCode[1] & 0xF0) >> 4);
            instr.OpCode += (ushort)(machineCode[0] << 4);

            instr.HighLowBit = (machineCode[1] & 0x8) != 0;

            instr.DestinationRegister = (byte)(machineCode[1] & 0x7);

            return instr;
        }

        public static R1Instruction ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 2)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            var instr = new R1Instruction();

            instr.Mnemonic = tokens[0];

            instr.DestinationRegister = GetRegister.FromString(tokens[1]);

            return instr;
        }
    }
}
