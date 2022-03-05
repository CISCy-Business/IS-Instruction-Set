using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Instructions.R3Types
{
    public class LogicShiftLeft : ArithmeticShiftLeft
    {
        public new const string Mnemonic = "LSL";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }
    }
}
