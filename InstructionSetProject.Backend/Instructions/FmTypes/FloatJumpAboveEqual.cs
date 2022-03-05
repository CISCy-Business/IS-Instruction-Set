using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions.FmTypes
{
    public class FloatJumpAboveEqual : FloatJumpNoCarry
    {
        public new const string Mnemonic = "JAE";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }
    }
}
