﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.R2Types
{
    public class ExchangeLessEqual : R2Instruction
    {
        public const string Mnemonic = "XLE";

        public const ushort OpCode = 0b0100_0001_0100_0000;

        public override string GetMnemonic()
        {
            return Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return OpCode;
        }
    }
}
