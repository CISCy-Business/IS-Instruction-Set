using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions.FmTypes
{
    public class FloatJumpNotEqual : FloatJumpNoZero
    {
        public new const string Mnemonic = "JNE";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }
    }
}
