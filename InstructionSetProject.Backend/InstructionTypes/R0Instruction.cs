using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public class R0Instruction : IInstruction
    {
        public string Mnemonic = "";
        public ushort OpCode;

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
            firstByte = (byte) (GetOpCode() >> 8);
            machineCode.Add(firstByte);

            byte secondByte = 0;
            secondByte = (byte) (GetOpCode() & 0xFF);
            machineCode.Add(secondByte);

            return machineCode;
        }

        public string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();

            return assembly;
        }

        public static R0Instruction ParseInstruction(List<byte> machineCode)
        {
            if (machineCode.Count != 2)
                throw new Exception("Incorrect number of bytes for this instruction type");

            var instr = new R0Instruction();

            instr.OpCode = machineCode[1];
            instr.OpCode += (ushort)(machineCode[0] << 4);

            return instr;
        }

        public static R0Instruction ParseInstruction(string assemblyCode)
        {
            var instr = new R0Instruction();

            instr.Mnemonic = assemblyCode.Trim();

            return instr;
        }
    }
}
