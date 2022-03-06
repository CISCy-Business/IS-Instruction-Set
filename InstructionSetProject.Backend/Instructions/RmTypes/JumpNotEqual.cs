using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions.RmTypes
{
    public class JumpNotEqual : JumpNoZero
    {
        public new const string Mnemonic = "JNE";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }
    }
}
