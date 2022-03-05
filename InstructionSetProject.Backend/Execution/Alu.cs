using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.Execution
{
    public class Alu
    {
        private const ushort signBit = 0b1000_0000_0000_0000;

        private StaticPipelineDataStructures dataStructures;

        public Alu(StaticPipelineDataStructures dataStructures)
        {
            this.dataStructures = dataStructures;
        }

        public (ushort result, FlagsRegister flags) Execute(AluOperation operation, ushort? lhsOp, ushort? rhsOp)
        {
            switch (operation)
            {
                case AluOperation.Add:
                    return UshortAdd(lhsOp, rhsOp);
                case AluOperation.Subtract:
                    return UshortSubtract(lhsOp, rhsOp);
                default:
                    throw new Exception("ALU operation not found");
            }
        }

        public (ushort result, FlagsRegister flags) UshortAdd(ushort? lhsOp, ushort? rhsOp)
        {
            var lhs = lhsOp ?? 0;
            var rhs = rhsOp ?? 0;
            var result = (ushort)(lhs + rhs);
            var flags = ComputeFlagsRegister(lhs, rhs, result);

            if (((lhs & signBit) != 0 || (rhs & signBit) != 0) && (result & signBit) == 0)
                flags.Carry = true;
            else
                flags.Carry = false;

            if (((lhs & signBit) != 0 && (rhs & signBit) != 0) && (result & signBit) == 0)
                flags.Overflow = true;
            else if (((lhs & signBit) == 0 && (rhs & signBit) == 0) && (result & signBit) != 0)
                flags.Overflow = true;
            else
                flags.Overflow = false;

            return (result, flags);
        }

        public (ushort result, FlagsRegister flags) UshortSubtract(ushort? lhsOp, ushort? rhsOp)
        {
            var lhs = lhsOp ?? 0;
            var rhs = rhsOp ?? 0;
            var result = (ushort) (lhs - rhs);
            var flags = ComputeFlagsRegister(lhs, rhs, result);

            if (((lhs & signBit) == 0 && (result & signBit) == 1))
                flags.Carry = true;
            else
                flags.Carry = false;

            flags.Overflow = false;

            return (result, flags);
        }

        private FlagsRegister ComputeFlagsRegister(ushort lhs, ushort rhs, ushort result)
        {
            FlagsRegister flags = new();

            flags.Zero = result == 0;
            flags.Parity = true;
            for (int i = 1; i < ushort.MaxValue; i = i << 1)
            {
                if ((result & i) != 0)
                {
                    flags.Parity = !flags.Parity;
                }
            }

            flags.Sign = (result & signBit) != 0;
            // Carry and overflow need to be calculated based on the operation

            return flags;
        }
    }

    public enum AluOperation
    {
        Add,
        Subtract
    }
}
