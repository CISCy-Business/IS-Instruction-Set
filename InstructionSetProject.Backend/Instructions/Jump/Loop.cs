using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.Instructions.Jump
{
    public class Loop : JumpInstruction
    {
        public new const string Mnemonic = "LOP";

        public new const ushort OpCode = 0x187;

        public Loop(JumpInstruction instr)
        {
            base.OpCode = instr.OpCode;
            base.Mnemonic = instr.Mnemonic;
            DestinationRegister = instr.DestinationRegister;
            HighLowBit = instr.HighLowBit;
            SourceRegister = instr.SourceRegister;
            Immediate = instr.Immediate;
        }

        public override string GetMnemonic()
        {
            return Loop.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return Loop.OpCode;
        }

        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += GetRegister.FromByte(DestinationRegister);
            assembly += ", ";
            assembly += Immediate.ToString("X2");

            return assembly;
        }

        public new static Loop ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 3)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            var instr = new JumpInstruction();

            instr.Mnemonic = tokens[0];

            instr.DestinationRegister = GetRegister.FromString(tokens[1].TrimEnd(','));

            instr.SourceRegister = 0;

            instr.HighLowBit = false;

            instr.Immediate = ushort.Parse(tokens[2]);

            return new Loop(instr);
        }
    }
}
