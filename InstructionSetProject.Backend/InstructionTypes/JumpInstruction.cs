using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public class JumpInstruction : IInstruction
    {
        public string Mnemonic = "";
        public ushort OpCode;
        public bool HighLowBit = false;
        public byte DestinationRegister;
        public byte SourceRegister;
        public ushort Immediate;

        public virtual string GetMnemonic()
        {
            return Mnemonic;
        }

        public virtual ushort GetOpCode()
        {
            return OpCode;
        }

        public List<byte> Assemble()
        {
            var machineCode = new List<byte>();

            byte firstByte = 0;
            firstByte += (byte) (GetOpCode() >> 1);
            machineCode.Add(firstByte);

            byte secondByte = 0;
            secondByte += (byte) ((GetOpCode() & 1) << 7);
            secondByte += (byte) (SourceRegister << 4);
            if (HighLowBit) secondByte += 8;
            secondByte += DestinationRegister;
            machineCode.Add(secondByte);

            byte thirdByte = 0;
            thirdByte += (byte) (Immediate >> 8);
            machineCode.Add(thirdByte);

            byte fourthByte = 0;
            fourthByte += (byte) (Immediate & 0xFF);
            machineCode.Add(fourthByte);

            return machineCode;
        }

        public virtual string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += GetRegister.FromByte(DestinationRegister);
            assembly += ", ";
            assembly += GetRegister.FromByte(SourceRegister);
            assembly += ", ";
            assembly += Immediate.ToString();

            return assembly;
        }

        public static JumpInstruction ParseInstruction(List<byte> machineCode)
        {
            if (machineCode.Count != 4)
                throw new Exception("Incorrect number of bytes for this instruction type");

            var instr = new JumpInstruction();

            instr.OpCode = (ushort)(machineCode[0] << 1);
            instr.OpCode += (ushort)((machineCode[1] & 0x128) >> 7);

            instr.DestinationRegister = (byte)(machineCode[1] & 0x7);

            instr.HighLowBit = (machineCode[1] & 0x8) != 0;

            instr.SourceRegister = (byte)((machineCode[1] & 0x70) >> 4);

            instr.Immediate = machineCode[3];
            instr.Immediate += (ushort)(machineCode[2] << 8);

            return instr;
        }

        public static JumpInstruction ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 4)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            var instr = new JumpInstruction();

            instr.Mnemonic = tokens[0];

            instr.DestinationRegister = GetRegister.FromString(tokens[1].TrimEnd(','));

            instr.SourceRegister = GetRegister.FromString(tokens[2].TrimEnd(','));

            instr.Immediate = ushort.Parse(tokens[2].TrimEnd(','));

            return instr;
        }
    }
}
