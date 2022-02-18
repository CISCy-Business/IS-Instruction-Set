using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.JumpTypes
{
    public class Return : JumpInstruction
    {
        public new const string Mnemonic = "RET";

        public new const ushort OpCode = 0x1C1;

        public Return(JumpInstruction instr)
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
            return Return.Mnemonic;
        }

        public override ushort GetOpCode()
        {
            return Return.OpCode;
        }

        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();

            return assembly;
        }

        public new static Return ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 1)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            var instr = new JumpInstruction();

            instr.Mnemonic = tokens[0];

            instr.DestinationRegister = 0;

            instr.SourceRegister = 0;

            instr.HighLowBit = false;

            instr.Immediate = 0;

            return new Return(instr);
        }
    }
}
