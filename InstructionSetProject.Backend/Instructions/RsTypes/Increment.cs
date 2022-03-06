using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.RsFormats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.RsTypes
{
    public class Increment : RsRegisterRegisterImmediate
    {
        public const string Mnemonic = "INC";

        public const ushort OpCode = 0b1000_0000_0000_0000;

        public override ControlBits controlBits => new(true, true, false, false, false, false, true);

        public override AluOperation? aluOperation => AluOperation.Add;

        public override string GetMnemonic()
        {
            return Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return OpCode;
        }
    }
}
