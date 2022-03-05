namespace InstructionSetProject.Backend.Execution
{
    public class Memory
    {
        public byte[] Bytes = new byte[1048576];

        public void Write(uint address, ushort value)
        {

        }

        public ushort Read(uint address)
        {
            throw new NotImplementedException();
        }

        public byte[] GetBytesAtAddress(uint address)
        {
            var arrayLength = 1000;
            byte[] result = new byte[arrayLength];
            Array.Copy(Bytes, address, result, 0, arrayLength);
            return result;
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
