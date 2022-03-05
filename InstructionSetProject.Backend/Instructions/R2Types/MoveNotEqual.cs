using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions.R2Types
{
    public class MoveNotEqual : MoveNoZero
    {
        public new const string Mnemonic = "MNE";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }
    }
}
