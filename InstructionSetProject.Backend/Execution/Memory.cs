namespace InstructionSetProject.Backend.Execution
{
    public class Memory
    {
        public byte[] Bytes = new byte[1048576];

        public void Write(ushort address, ushort value)
        {

        }

        public ushort Read(ushort address)
        {
            throw new NotImplementedException();
        }

        public void AddInstructionCode(List<byte> machineCode)
        {
            for (int i = 0; i < machineCode.Count; i++)
            {
                Bytes[i] = machineCode[i];
            }
        }
    }
}
