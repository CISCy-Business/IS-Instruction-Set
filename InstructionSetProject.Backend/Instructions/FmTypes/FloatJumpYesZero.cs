using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.InstructionTypes.FmFormats;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.Instructions.FmTypes
{
    public class FloatJumpYesZero : FmRegisterRegisterLabel, IFlagInstruction
    {
        public const string Mnemonic = "JYZ";

        public const ushort OpCode = 0b1110_1110_1000_0000;

        public override ControlBits controlBits => new(false, false, false, false, false, true, false);

        public override AluOperation? aluOperation => AluOperation.FloatSubtract;

        public Flags flagToCheck => Flags.Zero;

        public bool flagEnabled => true;

        public override int cyclesNeededInExecute => 2;

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
