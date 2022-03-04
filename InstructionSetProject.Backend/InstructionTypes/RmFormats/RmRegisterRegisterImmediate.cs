using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.InstructionTypes.RmFormats
{
    public abstract class RmRegisterRegisterImmediate : RmInstruction, IImmediateInstruction
    {
        public override ushort? addressingMode { get => sourceRegister1; set {} }

        public override string Disassemble()
        {
            string assembly = "";

            assembly += GetMnemonic();
            assembly += " ";
            assembly += Registers.ParseIntDestination(destinationRegister ?? 0);
            assembly += ", ";
            assembly += Registers.ParseIntFirstSource(sourceRegister1 ?? 0);
            assembly += ", ";
            assembly += (immediate ?? 0).ToString("X2");

            return assembly;
        }

        public override void ParseInstruction(string assemblyCode)
        {
            var tokens = assemblyCode.Split(' ');

            if (tokens.Length != 4)
                throw new Exception("Incorrect number of tokens obtained from assembly instruction");

            destinationRegister = Registers.ParseIntDestination(tokens[1].TrimEnd(','));

            sourceRegister1 = Registers.ParseIntFirstSource(tokens[2].TrimEnd(','));

            immediate = Convert.ToUInt16(tokens[3], 16);
        }

        public ushort GenerateImmediate()
        {
            return immediate ?? 0;
        }
    }
}
