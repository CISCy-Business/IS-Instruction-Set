﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.General
{
    public class LoadWord : ImmediateInstruction
    {
        public new const string Mnemonic = "LDW";

        public new const ushort OpCode = 0x80;

        public LoadWord(ImmediateInstruction instr)
        {
            base.OpCode = instr.OpCode;
            base.Mnemonic = instr.Mnemonic;
            DestinationRegister = instr.DestinationRegister;
            AddressingMode = instr.AddressingMode;
            HighLowBit = instr.HighLowBit;
            Immediate = instr.Immediate;
        }

        public override string GetMnemonic()
        {
            return LoadWord.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return LoadWord.OpCode;
        }
    }
}