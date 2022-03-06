using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions.F2Types
{
    public class FloatMoveBelowThan : FloatMoveYesCarry
    {
        public new const string Mnemonic = "MBT";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }
    }
}
