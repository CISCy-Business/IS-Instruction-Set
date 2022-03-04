using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend
{
    public static class Assembler
    {
        public static List<byte> Assemble(string assemblyCode) =>
            Assemble(GenerateInstructionList.FromString(assemblyCode));

        public static List<byte> Assemble(InstructionList instructions)
        {
            var machineCode = new List<byte>();
            foreach (var instr in instructions.Instructions)
            {
                var machineLine = AssembleInstruction(instr);

                foreach (var codePiece in machineLine)
                {
                    machineCode.Add((byte)(codePiece >> 8));
                    machineCode.Add((byte)(codePiece >> 0));
                }
            }
            return machineCode;
        }

        public static List<ushort> AssembleInstruction(IInstruction instr)
        {
            if (instr is ICISCInstruction)
            {
                return ((ICISCInstruction)instr).CISCAssemble();
            }

            var normalAssembly = instr.Assemble();
            var assemblyReturn = new List<ushort>() { normalAssembly.opcode };
            if (normalAssembly.operand != null)
                assemblyReturn.Add((ushort)normalAssembly.operand);
            return assemblyReturn;
        }
    }
}
