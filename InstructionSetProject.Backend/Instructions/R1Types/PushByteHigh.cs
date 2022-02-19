﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R1Types
{
    public class PushByteHigh : R1Instruction
    {
        public const string Mnemonic = "PSH";

        public const ushort OpCode = 0b0010_0000_0010_0000;

        public override string GetMnemonic()
        {
            return PushByteHigh.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return PushByteHigh.OpCode;
        }
    }
}
