using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Instructions.R2ITypes;
using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.Instructions.JumpTypes
{
    public class Loop : JumpInstruction, ICISCInstruction
    {
        public const string Mnemonic = "LOP";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }

        public override ushort GetOpCode()
        {
            throw new Exception("The Loop instruction does not have an op code as it is a CISC instruction.");
        }

        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Registers.ParseDestination(DestinationRegister);
            assembly += ", ";
            assembly += Immediate.ToString("X2");

            return assembly;
        }

        public override (ushort opcode, ushort? operand) Assemble()
        {
            throw new Exception("The Loop instruction does not have an op code as it is a CISC instruction.");
        }

        public List<ushort> CISCAssemble()
        {
            // Loop instruction:
            // SBI DestinationRegister, DestinationRegister, 1
            // JNZ r0, r0, Immediate

            var subtractImmediate = new BitwiseSubtractImmediate();
            subtractImmediate.DestinationRegister = DestinationRegister;
            subtractImmediate.SourceRegister = (ushort)(DestinationRegister << 3);
            subtractImmediate.Immediate = 1;

            var jumpNotZero = new JumpNotZero();
            jumpNotZero.DestinationRegister = Registers.ParseDestination("R0");
            jumpNotZero.SourceRegister = Registers.ParseFirstSource("R0");
            jumpNotZero.Immediate = Immediate;

            var subtractCode = subtractImmediate.Assemble();
            var jumpCode = jumpNotZero.Assemble();

            var machineCode = new List<ushort>(){ subtractCode.opcode, (ushort)subtractCode.operand, jumpCode.opcode, (ushort)jumpCode.operand };

            return machineCode;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 3)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            DestinationRegister = Registers.ParseDestination(tokens[1].Trim(','));

            SourceRegister = 0;

            Immediate = Convert.ToInt16(tokens[2], 16);
        }
    }
}
