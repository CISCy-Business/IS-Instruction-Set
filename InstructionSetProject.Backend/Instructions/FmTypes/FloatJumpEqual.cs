using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions.FmTypes
{
    public class FloatJumpEqual : FloatJumpYesZero
    {
        public new const string Mnemonic = "JEQ";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }
    }
}
