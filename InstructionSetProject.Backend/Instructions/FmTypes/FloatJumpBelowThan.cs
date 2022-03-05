using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions.FmTypes
{
    public class FloatJumpBelowThan : FloatJumpYesCarry
    {
        public new const string Mnemonic = "JBT";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }
    }
}
