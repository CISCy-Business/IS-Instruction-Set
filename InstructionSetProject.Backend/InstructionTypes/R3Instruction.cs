using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public class R3Instruction : IInstruction
    {
        public string Mnemonic = "";
        public ushort OpCode;
        public byte DestinationRegister;
        public byte SourceRegister1;
        public byte SourceRegister2;

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
            firstByte += (byte) (GetOpCode() << 1);
            firstByte += (byte) ((SourceRegister2 & 4) >> 2);
            machineCode.Add(firstByte);

            byte secondByte = 0;
            secondByte += (byte) ((SourceRegister2 & 3) << 6);
            secondByte += (byte) (SourceRegister1 << 3);
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
            assembly += GetRegister.FromByte(SourceRegister1);
            assembly += ", ";
            assembly += GetRegister.FromByte(SourceRegister2);

            return assembly;
        }

        public static R3Instruction ParseInstruction(List<byte> machineCode)
        {
            if (machineCode.Count != 2)
                throw new Exception("Incorrect number of bytes for this instruction type");

            var instr = new R3Instruction();

            instr.OpCode = (ushort)((machineCode[0] & 0xFE) >> 1);

            instr.DestinationRegister = (byte)(machineCode[1] & 0x7);

            instr.SourceRegister1 = (byte)((machineCode[1] & 0x38) >> 3);

            instr.SourceRegister2 = (byte)((machineCode[1] & 0xC0) >> 6);
            instr.SourceRegister2 += (byte)((machineCode[0] & 0x1) << 2);

            return instr;
        }

        public static R3Instruction ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 4)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            var instr = new R3Instruction();

            instr.Mnemonic = tokens[0];

            instr.DestinationRegister = GetRegister.FromString(tokens[1].TrimEnd(','));

            instr.SourceRegister1 = GetRegister.FromString(tokens[2].TrimEnd(','));

            instr.SourceRegister2 = GetRegister.FromString(tokens[3]);

            return instr;
        }

        public string GetAddressingModeString()
        {
            throw new NotImplementedException();
        }
    }
}
