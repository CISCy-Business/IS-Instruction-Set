using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.InstructionTypes.RmFormats;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.Instructions.RmTypes
{
    public class JumpYesCarry : RmRegisterRegisterLabel, IFlagInstruction
    {
        public const string Mnemonic = "JYC";

        public const ushort OpCode = 0b1100_1101_1000_0000;

        public override ControlBits controlBits => new(false, false, false, false, false, true, false);

        public override AluOperation? aluOperation => AluOperation.Subtract;

        public Flags flagToCheck => Flags.Carry;

        public bool flagEnabled => true;

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
