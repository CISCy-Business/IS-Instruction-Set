using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public class MemoryInstruction : IInstruction
    {
        public string Mnemonic = "";
        public ushort OpCode;
        public byte AddressingMode;
        public bool HighLowBit;
        public byte DestinationRegister;
        public ushort Immediate;

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

        public byte GetAddressingMode()
        {
            return AddressingMode;
        }

        public string GetAddressingModeString()
        {
            switch (AddressingMode)
            {
                case 0: return "i";
                case 1: return "d";
                case 2: return "dn";
                case 3: return "r";
                case 4: return "rn";
                case 5: return "xd";
                case 6: return "xn";
                case 7: return "xo";
                case 8: return "xf";
                case 9: return "sd";
                case 10: return "sn";
                case 11: return "so";
                case 12: return "sxd";
                case 13: return "sxn";
                case 14: return "sxo";
                case 15: return "sxf";
                default:
                    throw new Exception($"Addressing Mode Not Found: {AddressingMode}");
            }
        }

        public static byte ConvertAddressingModeToByte(string addrMode)
        {
            switch (addrMode)
            {
                case "i": return 0;
                case "d": return 1;
                case "dn": return 2;
                case "r": return 3;
                case "rn": return 4;
                case "xd": return 5;
                case "xn": return 6;
                case "xo": return 7;
                case "xf": return 8;
                case "sd": return 9;
                case "sn": return 10;
                case "so": return 11;
                case "sxd": return 12;
                case "sxn": return 13;
                case "sxo": return 14;
                case "sxf": return 15;
                default:
                    throw new Exception($"Addressing Mode Not Found: {addrMode}");
            }
        }

        public List<byte> Assemble()
        {
            var machineCode = new List<byte>();

            byte firstByte = 0;
            firstByte += (byte) (GetOpCode());
            machineCode.Add(firstByte);

            byte secondByte = 0;
            secondByte += (byte) (GetAddressingMode() << 4);
            if (GetHighLowBit()) secondByte += 8;
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

        public string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += GetRegister.FromByte(DestinationRegister);
            assembly += ", ";
            if (AddressingMode == '3' || AddressingMode == '4')
            {
                assembly += GetRegister.FromByte((byte)Immediate);
            }
            else
            {
                assembly += Immediate.ToString("X2");
            }
            assembly += ", ";
            assembly += GetAddressingModeString();

            return assembly;
        }

        public static MemoryInstruction ParseInstruction(List<byte> machineCode)
        {
            if (machineCode.Count != 4)
                throw new Exception("Incorrect number of bytes for this instruction type");

            var instr = new MemoryInstruction();

            instr.OpCode = machineCode[0];

            instr.AddressingMode = (byte)((machineCode[1] & 0xF0) >> 4);

            instr.HighLowBit = (machineCode[1] & 0x8) != 0;

            instr.DestinationRegister = (byte)(machineCode[1] & 0x7);

            instr.Immediate = machineCode[3];
            instr.Immediate += (ushort)(machineCode[2] << 8);

            return instr;
        }

        public static MemoryInstruction ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 4)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            var instr = new MemoryInstruction();

            instr.Mnemonic = tokens[0];

            instr.DestinationRegister = GetRegister.FromString(tokens[1].TrimEnd(','));

            if (tokens[2].StartsWith('r'))
            {
                instr.Immediate = GetRegister.FromString(tokens[2].TrimEnd(','));
            }
            else
            {
                instr.Immediate = Convert.ToUInt16(tokens[2].TrimEnd(','), 16);
            }

            instr.AddressingMode = MemoryInstruction.ConvertAddressingModeToByte(tokens[3]);

            return instr;
        }
    }
}
