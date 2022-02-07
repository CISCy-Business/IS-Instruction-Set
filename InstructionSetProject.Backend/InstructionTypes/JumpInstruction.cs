using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public class JumpInstruction : IInstruction
    {
        public string Mnemonic = "";
        public ushort OpCode;
        public bool HighLowBit;
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
            throw new NotImplementedException();
        }

        public string Disassemble()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public string GetAddressingModeString()
        {
            throw new NotImplementedException();
        }
    }
}
