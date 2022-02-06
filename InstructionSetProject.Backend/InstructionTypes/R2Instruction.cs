using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public class R2Instruction : IInstruction
    {
        public string Mnemonic = "";
        public ushort OpCode;
        public byte DestinationRegister;
        public byte SourceRegister;

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
            firstByte += (byte) (GetOpCode() >> 2);
            machineCode.Add(firstByte);

            byte secondByte = 0;
            secondByte += (byte) ((GetOpCode() & 0x3) << 6);
            secondByte += (byte) ((SourceRegister) << 3);
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
            assembly += ", ";
            assembly += GetRegister.FromByte(SourceRegister);

            return assembly;
        }

        public static R2Instruction ParseInstruction(List<byte> machineCode)
        {
            if (machineCode.Count != 2)
                throw new Exception("Incorrect number of bytes for this instruction type");

            var instr = new R2Instruction();

            instr.OpCode = (ushort)((machineCode[1] & 0xC0) >> 6);
            instr.OpCode += (ushort)(machineCode[0] << 2);

            instr.DestinationRegister = (byte)(machineCode[1] & 0x7);

            instr.SourceRegister = (byte)((machineCode[1] & 0x38) >> 3);

            return instr;
        }

        public static R2Instruction ParseInstruction(string assemblyCode)
        {
            throw new NotImplementedException();
        }
    }
}
