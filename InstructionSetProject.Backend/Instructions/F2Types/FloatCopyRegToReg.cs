using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions.F2Types
{
    public class FloatCopyRegToReg : FloatMoveUnconditional
    {
        public new const string Mnemonic = "CFF";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }
    }
}
