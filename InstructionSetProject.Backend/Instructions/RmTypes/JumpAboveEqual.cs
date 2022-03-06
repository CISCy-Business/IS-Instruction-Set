using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions.RmTypes
{
    public class JumpAboveEqual : JumpNoCarry
    {
        public new const string Mnemonic = "JAE";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }
    }
}
