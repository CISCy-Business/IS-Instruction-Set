using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes.R2Formats;
using InstructionSetProject.Backend.StaticPipeline;

namespace InstructionSetProject.Backend.Instructions.R2Types
{
    public class CopyRegToReg : MoveUnconditional
    {
        public new const string Mnemonic = "CRR";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }
    }
}
