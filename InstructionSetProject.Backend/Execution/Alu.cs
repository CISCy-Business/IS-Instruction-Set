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
                case AluOperation.AddWithCarry:
                    return UshortAddWithCarry(lhsOp, rhsOp);
                case AluOperation.Subtract:
                    return UshortSubtract(lhsOp, rhsOp);
                case AluOperation.SubtractWithBorrow:
                    return UshortSubtractWithBorrow(lhsOp, rhsOp);
                case AluOperation.FloatAdd:
                    return FloatAdd(lhsOp, rhsOp);
                case AluOperation.FloatAddWithCarry:
                    return FloatAddWithCarry(lhsOp, rhsOp);
                case AluOperation.FloatSubtract:
                    return FloatSubtract(lhsOp, rhsOp);
                case AluOperation.FloatSubtractWithBorrow:
                    return FloatSubtractWithBorrow(lhsOp, rhsOp);
                case AluOperation.BitwiseAnd:
                    return BitwiseAnd(lhsOp, rhsOp);
                case AluOperation.BitwiseOr:
                    return BitwiseOr(lhsOp, rhsOp);
                case AluOperation.BitwiseXor:
                    return BitwiseXor(lhsOp, rhsOp);
                case AluOperation.BitwiseNot:
                    return BitwiseNot(lhsOp, rhsOp);
                case AluOperation.Negate:
                    return Negate(lhsOp, rhsOp);
                case AluOperation.SetSignFlag:
                    return SetSignFlag(true);
                case AluOperation.ClearSignFlag:
                    return SetSignFlag(false);
                case AluOperation.SetParityFlag:
                    return SetParityFlag(true);
                case AluOperation.ClearParityFlag:
                    return SetParityFlag(false);
                case AluOperation.SetOverflowFlag:
                    return SetOverflowFlag(true);
                case AluOperation.ClearOverflowFlag:
                    return SetOverflowFlag(false);
                case AluOperation.SetCarryFlag:
                    return SetCarryFlag(true);
                case AluOperation.ClearCarryFlag:
                    return SetCarryFlag(false);
                case AluOperation.SetZeroFlag:
                    return SetZeroFlag(true);
                case AluOperation.ClearZeroFlag:
                    return SetZeroFlag(false);
                case AluOperation.AbsoluteValue:
                    return AbsoluteValue(lhsOp, rhsOp);
                case AluOperation.NoOperation:
                    return NoOperation();
                case AluOperation.PassFirstOperandThrough:
                    return PassFirstOperandThrough(lhsOp, rhsOp);
                case AluOperation.PassSecondOperandThrough:
                    return PassSecondOperandThrough(lhsOp, rhsOp);
                default:
                    throw new Exception("ALU operation not found");
            }
        }

        public (ushort result, FlagsRegister flags) UshortAdd(ushort? lhsOp, ushort? rhsOp)
        {
            var lhs = lhsOp ?? 0;
            var rhs = rhsOp ?? 0;
            var result = (ushort)(lhs + rhs);
            var flags = ComputeFlagsRegister(result);
            flags = CalculateAddFlags(lhs, rhs, result, flags);
            
            return (result, flags);
        }

        public (ushort result, FlagsRegister flags) UshortAddWithCarry(ushort? lhsOp, ushort? rhsOp)
        {
            var lhs = lhsOp ?? 0;
            var rhs = rhsOp ?? 0;
            var result = (ushort)(lhs + rhs + (dataStructures.Flags.Carry ? 1 : 0));
            var flags = ComputeFlagsRegister(result);
            flags = CalculateAddFlags(lhs, rhs, result, flags);

            return (result, flags);
        }

        public (ushort result, FlagsRegister flags) FloatAdd(ushort? lhsOp, ushort? rhsOp)
        {
            var lhsShort = lhsOp ?? 0;
            var rhsShort = rhsOp ?? 0;
            var lhs = (float)BitConverter.UInt16BitsToHalf(lhsShort);
            var rhs = (float) BitConverter.UInt16BitsToHalf(rhsShort);
            var result = (Half) (lhs + rhs);
            var resultShort = BitConverter.HalfToUInt16Bits(result);
            var flags = ComputeFlagsRegister(resultShort);
            flags = CalculateAddFlags(lhsShort, rhsShort, resultShort, flags);

            return (resultShort, flags);
        }

        public (ushort result, FlagsRegister flags) FloatAddWithCarry(ushort? lhsOp, ushort? rhsOp)
        {
            var lhsShort = lhsOp ?? 0;
            var rhsShort = rhsOp ?? 0;
            var lhs = (float)BitConverter.UInt16BitsToHalf(lhsShort);
            var rhs = (float)BitConverter.UInt16BitsToHalf(rhsShort);
            var result = (Half)(lhs + rhs + (dataStructures.Flags.Carry ? 1f : 0f));
            var resultShort = BitConverter.HalfToUInt16Bits(result);
            var flags = ComputeFlagsRegister(resultShort);
            flags = CalculateAddFlags(lhsShort, rhsShort, resultShort, flags);

            return (resultShort, flags);
        }

        private FlagsRegister CalculateAddFlags(ushort lhs, ushort rhs, ushort result, FlagsRegister flags)
        {
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

            return flags;
        }

        public (ushort result, FlagsRegister flags) UshortSubtract(ushort? lhsOp, ushort? rhsOp)
        {
            var lhs = lhsOp ?? 0;
            var rhs = rhsOp ?? 0;
            var result = (ushort) (lhs - rhs);
            var flags = ComputeFlagsRegister(result);
            flags = CalculateSubtractFlags(lhs, rhs, result, flags);

            return (result, flags);
        }

        public (ushort result, FlagsRegister flags) UshortSubtractWithBorrow(ushort? lhsOp, ushort? rhsOp)
        {
            var lhs = lhsOp ?? 0;
            var rhs = rhsOp ?? 0;
            var result = (ushort)(lhs - rhs - (dataStructures.Flags.Carry ? 1 : 0));
            var flags = ComputeFlagsRegister(result);
            flags = CalculateSubtractFlags(lhs, rhs, result, flags);

            return (result, flags);
        }

        public (ushort result, FlagsRegister flags) FloatSubtract(ushort? lhsOp, ushort? rhsOp)
        {
            var lhsShort = lhsOp ?? 0;
            var rhsShort = rhsOp ?? 0;
            var lhs = (float)BitConverter.UInt16BitsToHalf(lhsShort);
            var rhs = (float)BitConverter.UInt16BitsToHalf(rhsShort);
            var result = (Half)(lhs - rhs);
            var resultShort = BitConverter.HalfToUInt16Bits(result);
            var flags = ComputeFlagsRegister(resultShort);
            flags = CalculateSubtractFlags(lhsShort, rhsShort, resultShort, flags);

            return (resultShort, flags);
        }

        public (ushort result, FlagsRegister flags) FloatSubtractWithBorrow(ushort? lhsOp, ushort? rhsOp)
        {
            var lhsShort = lhsOp ?? 0;
            var rhsShort = rhsOp ?? 0;
            var lhs = (float)BitConverter.UInt16BitsToHalf(lhsShort);
            var rhs = (float)BitConverter.UInt16BitsToHalf(rhsShort);
            var result = (Half)(lhs - rhs - (dataStructures.Flags.Carry ? 1f : 0f));
            var resultShort = BitConverter.HalfToUInt16Bits(result);
            var flags = ComputeFlagsRegister(resultShort);
            flags = CalculateSubtractFlags(lhsShort, rhsShort, resultShort, flags);

            return (resultShort, flags);
        }

        private FlagsRegister CalculateSubtractFlags(ushort lhs, ushort rhs, ushort result, FlagsRegister flags)
        {
            if (((lhs & signBit) == 0 && (result & signBit) != 0))
                flags.Carry = true;
            else
                flags.Carry = false;

            flags.Overflow = false;

            return flags;
        }

        public (ushort result, FlagsRegister flags) BitwiseAnd(ushort? lhsOp, ushort? rhsOp)
        {
            var lhs = lhsOp ?? 0;
            var rhs = rhsOp ?? 0;
            var result = (ushort)(lhs & rhs);
            var flags = ComputeFlagsRegister(result);

            return (result, flags);
        }

        public (ushort result, FlagsRegister flags) BitwiseOr(ushort? lhsOp, ushort? rhsOp)
        {
            var lhs = lhsOp ?? 0;
            var rhs = rhsOp ?? 0;
            var result = (ushort)(lhs | rhs);
            var flags = ComputeFlagsRegister(result);

            return (result, flags);
        }

        public (ushort result, FlagsRegister flags) BitwiseXor(ushort? lhsOp, ushort? rhsOp)
        {
            var lhs = lhsOp ?? 0;
            var rhs = rhsOp ?? 0;
            var result = (ushort)(lhs ^ rhs);
            var flags = ComputeFlagsRegister(result);

            return (result, flags);
        }

        public (ushort result, FlagsRegister flags) BitwiseNot(ushort? lhsOp, ushort? rhsOp)
        {
            var lhs = lhsOp ?? 0;
            var result = (ushort)(~lhs);
            var flags = ComputeFlagsRegister(result);

            return (result, flags);
        }

        public (ushort result, FlagsRegister flags) Negate(ushort? lhsOp, ushort? rhsOp)
        {
            var lhs = lhsOp ?? 0;
            var result = (ushort) (lhs * -1);
            var flags = ComputeFlagsRegister(result);

            return (result, flags);
        }

        public (ushort result, FlagsRegister flags) SetSignFlag(bool value)
        {
            dataStructures.Flags.Sign = value;
            return (0, dataStructures.Flags);
        }

        public (ushort result, FlagsRegister flags) SetParityFlag(bool value)
        {
            dataStructures.Flags.Parity = value;
            return (0, dataStructures.Flags);
        }

        public (ushort result, FlagsRegister flags) SetOverflowFlag(bool value)
        {
            dataStructures.Flags.Overflow = value;
            return (0, dataStructures.Flags);
        }

        public (ushort result, FlagsRegister flags) SetCarryFlag(bool value)
        {
            dataStructures.Flags.Carry = value;
            return (0, dataStructures.Flags);
        }

        public (ushort result, FlagsRegister flags) SetZeroFlag(bool value)
        {
            dataStructures.Flags.Zero = value;
            return (0, dataStructures.Flags);
        }

        public (ushort result, FlagsRegister flags) AbsoluteValue(ushort? lhsOp, ushort? rhsOp)
        {
            var lhs = lhsOp ?? 0;
            var result = (ushort) (lhs & 0b0111_1111_1111_1111);
            var flags = ComputeFlagsRegister(result);

            return (result, flags);
        }

        public (ushort result, FlagsRegister flags) NoOperation()
        {
            return (0, dataStructures.Flags);
        }

        public (ushort result, FlagsRegister flags) PassSecondOperandThrough(ushort? lhsOp, ushort? rhsOp)
        {
            return (rhsOp ?? 0, dataStructures.Flags);
        }

        public (ushort result, FlagsRegister flags) PassFirstOperandThrough(ushort? lhsOp, ushort? rhsOp)
        {
            return (lhsOp ?? 0, dataStructures.Flags);
        }

        private FlagsRegister ComputeFlagsRegister(ushort result)
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
        AddWithCarry,
        Subtract,
        SubtractWithBorrow,
        FloatAdd,
        FloatAddWithCarry,
        FloatSubtract,
        FloatSubtractWithBorrow,
        BitwiseAnd,
        BitwiseOr,
        BitwiseXor,
        BitwiseNot,
        Negate,
        SetSignFlag,
        ClearSignFlag,
        SetParityFlag,
        ClearParityFlag,
        SetOverflowFlag,
        ClearOverflowFlag,
        SetCarryFlag,
        ClearCarryFlag,
        SetZeroFlag,
        ClearZeroFlag,
        AbsoluteValue,
        NoOperation,
        PassFirstOperandThrough,
        PassSecondOperandThrough
    }
}
