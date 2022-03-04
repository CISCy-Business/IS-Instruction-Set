namespace InstructionSetProject.Backend.StaticPipeline
{
    public class ControlBits
    {
        public bool RegWrite;
        public bool ALUSrc;
        public bool MemRead;
        public bool MemWrite;
        public bool MemToReg;
        public bool PCSrc;
        public bool UpdateFlags;

        public ControlBits(bool regWrite, bool aluSrc, bool memRead, bool memWrite, bool memToReg, bool pcSrc, bool updateFlags)
        {
            RegWrite = regWrite;
            ALUSrc = aluSrc;
            MemRead = memRead;
            MemWrite = memWrite;
            MemToReg = memToReg;
            PCSrc = pcSrc;
            UpdateFlags = updateFlags;
        }
    }
}
