using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions.R2Types
{
    public class MoveAboveEqual : MoveNoCarry
    {
        public new const string Mnemonic = "MAE";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }
    }
}
