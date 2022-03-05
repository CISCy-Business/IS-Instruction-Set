using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions.RmTypes
{
    public class JumpEqual : JumpYesZero
    {
        public new const string Mnemonic = "JEQ";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }
    }
}
