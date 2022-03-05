﻿using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.InstructionTypes.RmFormats;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.Instructions.RmTypes
{
    public class JumpNoZero : RmRegisterRegisterLabel, IFlagInstruction
    {
        public const string Mnemonic = "JNZ";

        public const ushort OpCode = 0b1100_1111_0000_0000;

        public override ControlBits controlBits => new(false, false, false, false, false, true, false);

        public override AluOperation? aluOperation => AluOperation.Subtract;

        public Flags flagToCheck => Flags.Zero;

        public bool flagEnabled => false;

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
