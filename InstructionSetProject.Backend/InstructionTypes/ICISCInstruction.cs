﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.InstructionTypes
{
    public interface ICISCInstruction
    {
        public List<ushort> CISCAssemble();
    }
}