using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions.F2Types
{
    public class FloatMoveNotEqual : FloatMoveNoZero
    {
        public new const string Mnemonic = "MNE";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }
    }
}
