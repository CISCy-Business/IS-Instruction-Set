using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Instructions.R0Types;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Instructions.JumpTypes
{
    public class Call : JumpInstruction, ICISCInstruction
    {
        public const string Mnemonic = "CAL";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }

        public override ushort GetOpCode()
        {
            throw new Exception("The Call instruction does not have an op code as it is a CISC instruction.");
        }

        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Immediate.ToString("X2");

            return assembly;
        }

        public override (ushort opcode, ushort? operand) Assemble()
        {
            throw new Exception("The Call instruction does not have an op code as it is a CISC instruction.");
        }

        public List<ushort> CISCAssemble()
        {
            // Call instruction:
            // PUPC
            // JMP Immediate

            var pushPC = new PushPC();

            var jmp = new JumpUnconditional();
            jmp.DestinationRegister = 0;
            jmp.SourceRegister = 0;
            jmp.Immediate = Immediate;

            var pushCode = pushPC.Assemble();
            var jumpCode = jmp.Assemble();

            var machineCode = new List<ushort>() { pushCode.opcode, jumpCode.opcode, (ushort)jumpCode.operand };

            return machineCode;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 2)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            DestinationRegister = 0;

            SourceRegister = 0;

            Immediate = Convert.ToInt16(tokens[1], 16);
        }
    }
}
