using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.Instructions.R3Types
{
    public class MoveRegister : R3Instruction
    {
        public const string Mnemonic = "MOV";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }

        public override ushort GetOpCode()
        {
            // Instruction is an alias for Bitwise Add. Return the Add op code instead
            return BitwiseAdd.OpCode;
        }

        public override string Disassemble()
        {
            string assembly = "";


            assembly += GetMnemonic();
            assembly += " ";
            assembly += Registers.ParseDestination(DestinationRegister);
            assembly += ", ";
            assembly += Registers.ParseFirstSource(SourceRegister1);

            return assembly;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 3)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            DestinationRegister = Registers.ParseDestination(tokens[1].TrimEnd(','));

            SourceRegister1 = Registers.ParseFirstSource(tokens[2]);

            SourceRegister2 = Registers.ParseSecondSource("R0");
        }
    }
}
