using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.Instructions.R2ITypes
{
    public class MoveImmediate : R2IInstruction
    {
        public const string Mnemonic = "MVI";

        public override string GetMnemonic()
        {
            return Mnemonic;
        }

        public override ushort GetOpCode()
        {
            // Instruction is an alias for BitwiseAddImmediate, so return it's Op Code
            return BitwiseAddImmediate.OpCode;
        }

        public override string Disassemble()
        {
            string assembly = "";

            assembly = GetMnemonic();
            assembly += " ";
            assembly += Register.ParseDestination(DestinationRegister);
            assembly += ", ";
            assembly += Immediate.ToString("X2");

            return assembly;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 3)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            DestinationRegister = Register.ParseDestination(tokens[1].TrimEnd(','));

            SourceRegister = Register.ParseFirstSource("R0");

            Immediate = Convert.ToInt16(tokens[2], 16);
        }
    }
}
