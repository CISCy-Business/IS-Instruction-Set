using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.Jump
{
    public class Call : JumpInstruction
    {
        public new const string Mnemonic = "CAL";

        public new const ushort OpCode = 0x1C0;

        public Call(JumpInstruction instr)
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
            return Call.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return Call.OpCode;
        }

        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Immediate.ToString("X2");

            return assembly;
        }

        public new static Call ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 2)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            var instr = new JumpInstruction();

            instr.Mnemonic = tokens[0];

            instr.DestinationRegister = 0;

            instr.SourceRegister = 0;

            instr.HighLowBit = false;

            instr.Immediate = ushort.Parse(tokens[1]);

            return new Call(instr);
        }
    }
}
