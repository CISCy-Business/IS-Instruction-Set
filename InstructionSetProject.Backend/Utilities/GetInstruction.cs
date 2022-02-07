using InstructionSetProject.Backend.Instructions.Arithmetic;
using InstructionSetProject.Backend.Instructions.General;
using InstructionSetProject.Backend.Instructions.Jump;
using InstructionSetProject.Backend.Instructions.Stack;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Utilities
{
    public static class GetInstruction
    {
        public static IInstruction FromOpCode(IInstruction instr)
        {
            var opCode = instr.GetOpCode();
            switch (opCode)
            {
                case LoadWord.OpCode:
                    return new LoadWord((ImmediateInstruction)instr);
                case LoadByteHigh.OpCode:
                    if (((ImmediateInstruction) instr).HighLowBit)
                        return new LoadByteHigh((ImmediateInstruction) instr);
                    else
                        return new LoadByteLow((ImmediateInstruction) instr);
                case StoreWord.OpCode:
                    return new StoreWord((ImmediateInstruction)instr);
                case StoreByteHigh.OpCode:
                    if (((ImmediateInstruction)instr).HighLowBit)
                        return new StoreByteHigh((ImmediateInstruction)instr);
                    else
                        return new StoreByteLow((ImmediateInstruction)instr);
                case Halt.OpCode:
                    return new Halt((R0Instruction)instr);
                case NoOperation.OpCode:
                    return new NoOperation((R0Instruction)instr);
                case BitwiseAdd.OpCode:
                    return new BitwiseAdd((R3Instruction)instr);
                case BitwiseSubtract.OpCode:
                    return new BitwiseSubtract((R3Instruction)instr);
                case BitwiseAnd.OpCode:
                    return new BitwiseAnd((R3Instruction)instr);
                case BitwiseOr.OpCode:
                    return new BitwiseOr((R3Instruction)instr);
                case BitwiseXor.OpCode:
                    return new BitwiseXor((R3Instruction)instr);
                case BitwiseNot.OpCode:
                    return new BitwiseNot((R2Instruction)instr);
                case BitwiseNeg.OpCode:
                    return new BitwiseNeg((R2Instruction)instr);
                case ArithmeticShiftLeft.OpCode:
                    return new ArithmeticShiftLeft((R3Instruction)instr);
                case ArithmeticShiftRight.OpCode:
                    return new ArithmeticShiftRight((R3Instruction)instr);
                case LogicalShiftRight.OpCode:
                    return new LogicalShiftRight((R3Instruction)instr);
                case RotateLeft.OpCode:
                    return new RotateLeft((R3Instruction)instr);
                case RotateRight.OpCode:
                    return new RotateRight((R3Instruction)instr);
                case RotateLeftCarry.OpCode:
                    return new RotateLeftCarry((R3Instruction)instr);
                case RotateRightCarry.OpCode:
                    return new RotateRightCarry((R3Instruction)instr);
                case JumpUnconditional.OpCode:
                    return new JumpUnconditional((JumpInstruction)instr);
                case JumpLessThan.OpCode:
                    return new JumpLessThan((JumpInstruction)instr);
                case JumpGreaterThan.OpCode:
                    return new JumpGreaterThan((JumpInstruction)instr);
                case JumpLessEqual.OpCode:
                    return new JumpLessEqual((JumpInstruction)instr);
                case JumpGreaterEqual.OpCode:
                    return new JumpGreaterEqual((JumpInstruction)instr);
                case JumpEqual.OpCode:
                    return new JumpEqual((JumpInstruction)instr);
                case JumpNotEqual.OpCode:
                    return new JumpNotEqual((JumpInstruction)instr);
                case Loop.OpCode:
                    return new Loop((JumpInstruction)instr);
                case Call.OpCode:
                    return new Call((JumpInstruction)instr);
                case Return.OpCode:
                    return new Return((JumpInstruction)instr);
                case PushWord.OpCode:
                    return new PushWord((R1Instruction)instr);
                case PushByteHigh.OpCode:
                    if (((R1Instruction)instr).HighLowBit)
                        return new PushByteHigh((R1Instruction)instr);
                    else
                        return new PushByteLow((R1Instruction)instr);
                case PopWord.OpCode:
                    return new PopWord((R1Instruction)instr);
                case PopByteHigh.OpCode:
                    if (((R1Instruction)instr).HighLowBit)
                        return new PopByteHigh((R1Instruction)instr);
                    else
                        return new PopByteLow((R1Instruction)instr);
                default:
                    throw new Exception($"Instruction not found with OpCode: {opCode}");
            }
        }

        public static IInstruction FromMnemonic(IInstruction instr)
        {
            var mnemonic = instr.GetMnemonic();
            switch (mnemonic)
            {
                case LoadWord.Mnemonic:
                    return new LoadWord((ImmediateInstruction)instr);
                case LoadByteHigh.Mnemonic:
                    if (((ImmediateInstruction)instr).HighLowBit)
                        return new LoadByteHigh((ImmediateInstruction)instr);
                    else
                        return new LoadByteLow((ImmediateInstruction)instr);
                case StoreWord.Mnemonic:
                    return new StoreWord((ImmediateInstruction)instr);
                case StoreByteHigh.Mnemonic:
                    if (((ImmediateInstruction)instr).HighLowBit)
                        return new StoreByteHigh((ImmediateInstruction)instr);
                    else
                        return new StoreByteLow((ImmediateInstruction)instr);
                case Halt.Mnemonic:
                    return new Halt((R0Instruction)instr);
                case NoOperation.Mnemonic:
                    return new NoOperation((R0Instruction)instr);
                case BitwiseAdd.Mnemonic:
                    return new BitwiseAdd((R3Instruction)instr);
                case BitwiseSubtract.Mnemonic:
                    return new BitwiseSubtract((R3Instruction)instr);
                case BitwiseAnd.Mnemonic:
                    return new BitwiseAnd((R3Instruction)instr);
                case BitwiseOr.Mnemonic:
                    return new BitwiseOr((R3Instruction)instr);
                case BitwiseXor.Mnemonic:
                    return new BitwiseXor((R3Instruction)instr);
                case BitwiseNot.Mnemonic:
                    return new BitwiseNot((R2Instruction)instr);
                case BitwiseNeg.Mnemonic:
                    return new BitwiseNeg((R2Instruction)instr);
                case ArithmeticShiftLeft.Mnemonic:
                    return new ArithmeticShiftLeft((R3Instruction)instr);
                case ArithmeticShiftRight.Mnemonic:
                    return new ArithmeticShiftRight((R3Instruction)instr);
                case LogicalShiftRight.Mnemonic:
                    return new LogicalShiftRight((R3Instruction)instr);
                case RotateLeft.Mnemonic:
                    return new RotateLeft((R3Instruction)instr);
                case RotateRight.Mnemonic:
                    return new RotateRight((R3Instruction)instr);
                case RotateLeftCarry.Mnemonic:
                    return new RotateLeftCarry((R3Instruction)instr);
                case RotateRightCarry.Mnemonic:
                    return new RotateRightCarry((R3Instruction)instr);
                case JumpUnconditional.Mnemonic:
                    return new JumpUnconditional((JumpInstruction)instr);
                case JumpLessThan.Mnemonic:
                    return new JumpLessThan((JumpInstruction)instr);
                case JumpGreaterThan.Mnemonic:
                    return new JumpGreaterThan((JumpInstruction)instr);
                case JumpLessEqual.Mnemonic:
                    return new JumpLessEqual((JumpInstruction)instr);
                case JumpGreaterEqual.Mnemonic:
                    return new JumpGreaterEqual((JumpInstruction)instr);
                case JumpEqual.Mnemonic:
                    return new JumpEqual((JumpInstruction)instr);
                case JumpNotEqual.Mnemonic:
                    return new JumpNotEqual((JumpInstruction)instr);
                case Loop.Mnemonic:
                    return new Loop((JumpInstruction)instr);
                case Call.Mnemonic:
                    return new Call((JumpInstruction)instr);
                case Return.Mnemonic:
                    return new Return((JumpInstruction)instr);
                case PushWord.Mnemonic:
                    return new PushWord((R1Instruction)instr);
                case PushByteHigh.Mnemonic:
                    if (((R1Instruction)instr).HighLowBit)
                        return new PushByteHigh((R1Instruction)instr);
                    else
                        return new PushByteLow((R1Instruction)instr);
                case PopWord.Mnemonic:
                    return new PopWord((R1Instruction)instr);
                case PopByteHigh.Mnemonic:
                    if (((R1Instruction)instr).HighLowBit)
                        return new PopByteHigh((R1Instruction)instr);
                    else
                        return new PopByteLow((R1Instruction)instr);
                default:
                    throw new Exception($"Instruction not found with Mnemonic: {mnemonic}");
            }
        }
    }
}
