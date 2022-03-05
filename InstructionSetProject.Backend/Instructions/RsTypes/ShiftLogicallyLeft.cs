using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions.RsTypes
{
    public class ShiftLogicallyLeft : ShiftArithmeticallyLeft
    {
        public new const string Mnemonic = "SLL";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }
    }
}
