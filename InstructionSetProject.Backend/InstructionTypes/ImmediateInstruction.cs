using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<byte> Assemble()
        {
            throw new NotImplementedException();
        }

        public string Disassemble()
        {
            throw new NotImplementedException();
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
