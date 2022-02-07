using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public class ImmediateInstruction : IInstruction
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

        public virtual byte GetAddressingMode()
        {
            return AddressingMode;
        }

        public virtual string GetAddressingModeString()
        {
            switch (AddressingMode)
            {
                case 0: return "i  ";
                case 1: return "d  ";
                case 2: return "dn ";
                case 3: return "r  ";
                case 4: return "rn ";
                case 5: return "xd ";
                case 6: return "xn ";
                case 7: return "xo ";
                case 8: return "xf ";
                case 9: return "sd ";
                case 10: return "sn ";
                case 11: return "so ";
                case 12: return "sxd";
                case 13: return "sxn";
                case 14: return "sxo";
                case 15: return "sxf";
                default:
                    throw new Exception($"Address Not Found: {AddressingMode}");
            }
        }

        public List<byte> Assemble()
        {
            throw new NotImplementedException();
        }

        public string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += GetRegister.FromByte(DestinationRegister);
            assembly += ", ";
            assembly += GetRegister.FromByte(DestinationRegister);
            assembly += ", ";
            assembly += GetAddressingModeString();

            return assembly;

            //throw new NotImplementedException();
        }

        public static ImmediateInstruction ParseInstruction(List<byte> machineCode)
        {
            if (machineCode.Count != 4)
                throw new Exception("Incorrect number of bytes for this instruction type");

            var instr = new ImmediateInstruction();

            instr.OpCode = machineCode[0];

            instr.AddressingMode = (byte)((machineCode[1] & 0xF0) >> 4);

            instr.HighLowBit = (machineCode[1] & 0x8) != 0;

            instr.DestinationRegister = (byte)(machineCode[1] & 0x7);

            instr.Immediate = machineCode[3];
            instr.Immediate += (ushort)(machineCode[2] << 8);

            return instr;
        }

        public static ImmediateInstruction ParseInstruction(string assemblyCode)
        {
            throw new NotImplementedException();
        }
    }
}
