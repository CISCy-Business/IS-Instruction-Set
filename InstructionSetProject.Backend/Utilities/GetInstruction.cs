using InstructionSetProject.Backend.Instructions.JumpTypes;
using InstructionSetProject.Backend.Instructions.MemoryTypes;
using InstructionSetProject.Backend.Instructions.R0Types;
using InstructionSetProject.Backend.Instructions.R1Types;
using InstructionSetProject.Backend.Instructions.R2ITypes;
using InstructionSetProject.Backend.Instructions.R2Types;
using InstructionSetProject.Backend.Instructions.R3Types;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Utilities
{
    public static class GetInstruction
    {
        public static IInstruction FromOpCode(List<byte> machineCode)
        {
            if (machineCode.Count > 2)
                machineCode = machineCode.GetRange(0, 2);
            var instrCode = InstructionUtilities.ConvertToUshort(machineCode);

            if ((instrCode & JumpInstruction.BitwiseMask) == JumpEqual.OpCode)
                return new JumpEqual();

            if ((instrCode & MemoryInstruction.BitwiseMask) == LoadWord.OpCode)
                return new LoadWord();

            if ((instrCode & R0Instruction.BitwiseMask) == Halt.OpCode)
                return new Halt();

            if ((instrCode & R0Instruction.BitwiseMask) == NoOperation.OpCode)
                return new NoOperation();

            if ((instrCode & R1Instruction.BitwiseMask) == PopWord.OpCode)
                return new PopWord();

            if ((instrCode & R2Instruction.BitwiseMask) == BitwiseNeg.OpCode)
                return new BitwiseNeg();

            if ((instrCode & R2IInstruction.BitwiseMask) == BitwiseAddImmediate.OpCode)
                return new BitwiseAddImmediate();

            if ((instrCode & R3Instruction.BitwiseMask) == BitwiseAdd.OpCode)
                return new BitwiseAdd();

            throw new Exception("Instruction not found with given op code");
        }

        public static IInstruction FromMnemonic(string instruction)
        {
            var mnemonic = instruction.Split(' ')[0];

            switch (mnemonic)
            {
                case JumpEqual.Mnemonic:
                    return new JumpEqual();
                case LoadWord.Mnemonic:
                    return new LoadWord();
                case Halt.Mnemonic:
                    return new Halt();
                case NoOperation.Mnemonic:
                    return new NoOperation();
                case PopWord.Mnemonic:
                    return new PopWord();
                case BitwiseNeg.Mnemonic:
                    return new BitwiseNeg();
                case BitwiseAddImmediate.Mnemonic:
                    return new BitwiseAddImmediate();
                case BitwiseAdd.Mnemonic:
                    return new BitwiseAdd();
                default:
                    throw new Exception($"Instruction not found with given mnemonic: {mnemonic}");
            }
        }
    }
}
