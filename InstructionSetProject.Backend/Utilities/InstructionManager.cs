using InstructionSetProject.Backend.Instructions.F2Types;
using InstructionSetProject.Backend.Instructions.F3Types;
using InstructionSetProject.Backend.Instructions.FmTypes;
using InstructionSetProject.Backend.Instructions.R2Types;
using InstructionSetProject.Backend.Instructions.R3Types;
using InstructionSetProject.Backend.Instructions.RmTypes;
using InstructionSetProject.Backend.Instructions.RsTypes;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.Utilities
{
    public class InstructionManager
    {
        public static InstructionManager Singleton = new();

        private Dictionary<ushort, Func<IInstruction>> opcodeDictionary = new();
        private Dictionary<string, Func<IInstruction>> normalMnemonicDictionary = new();
        private Dictionary<string, Func<IInstruction>> floatMnemonicDictionary = new();

        public InstructionManager()
        {
            // F2 Instructions

            AddFloat(Ceiling.OpCode, Ceiling.Mnemonic, () => new Ceiling());
            AddFloat(FloatAbsoluteValue.OpCode, FloatAbsoluteValue.Mnemonic, () => new FloatAbsoluteValue());
            AddFloat(FloatCompare.OpCode, FloatCompare.Mnemonic, () => new FloatCompare());
            AddFloat(null, FloatCopyRegToReg.Mnemonic, () => new FloatCopyRegToReg());
            AddFloat(FloatExchangeRegisters.OpCode, FloatExchangeRegisters.Mnemonic, () => new FloatExchangeRegisters());
            AddFloat(null, FloatMoveAboveEqual.Mnemonic, () => new FloatMoveAboveEqual());
            AddFloat(FloatMoveAboveThan.OpCode, FloatMoveAboveThan.Mnemonic, () => new FloatMoveAboveThan());
            AddFloat(FloatMoveBelowEqual.OpCode, FloatMoveBelowEqual.Mnemonic, () => new FloatMoveBelowEqual());
            AddFloat(null, FloatMoveBelowThan.Mnemonic, () => new FloatMoveBelowThan());
            AddFloat(null, FloatMoveEqual.Mnemonic, () => new FloatMoveEqual());
            AddFloat(FloatMoveGreaterEqual.OpCode, FloatMoveGreaterEqual.Mnemonic, () => new FloatMoveGreaterEqual());
            AddFloat(FloatMoveGreaterThan.OpCode, FloatMoveGreaterThan.Mnemonic, () => new FloatMoveGreaterThan());
            AddFloat(FloatMoveLessEqual.OpCode, FloatMoveLessEqual.Mnemonic, () => new FloatMoveLessEqual());
            AddFloat(FloatMoveLessThan.OpCode, FloatMoveLessThan.Mnemonic, () => new FloatMoveLessThan());
            AddFloat(FloatMoveNoCarry.OpCode, FloatMoveNoCarry.Mnemonic, () => new FloatMoveNoCarry());
            AddFloat(FloatMoveNoOverflow.OpCode, FloatMoveNoOverflow.Mnemonic, () => new FloatMoveNoOverflow());
            AddFloat(FloatMoveNoParity.OpCode, FloatMoveNoParity.Mnemonic, () => new FloatMoveNoParity());
            AddFloat(FloatMoveNoSign.OpCode, FloatMoveNoSign.Mnemonic, () => new FloatMoveNoSign());
            AddFloat(null, FloatMoveNotEqual.Mnemonic, () => new FloatMoveNotEqual());
            AddFloat(FloatMoveNoZero.OpCode, FloatMoveNoZero.Mnemonic, () => new FloatMoveNoZero());
            AddFloat(FloatMoveUnconditional.OpCode, FloatMoveUnconditional.Mnemonic, () => new FloatMoveUnconditional());
            AddFloat(FloatMoveYesCarry.OpCode, FloatMoveYesCarry.Mnemonic, () => new FloatMoveYesCarry());
            AddFloat(FloatMoveYesOverflow.OpCode, FloatMoveYesOverflow.Mnemonic, () => new FloatMoveYesOverflow());
            AddFloat(FloatMoveYesParity.OpCode, FloatMoveYesParity.Mnemonic, () => new FloatMoveYesParity());
            AddFloat(FloatMoveYesSign.OpCode, FloatMoveYesSign.Mnemonic, () => new FloatMoveYesSign());
            AddFloat(FloatMoveYesZero.OpCode, FloatMoveYesZero.Mnemonic, () => new FloatMoveYesZero());
            AddFloat(FloatNegate.OpCode, FloatNegate.Mnemonic, () => new FloatNegate());
            AddFloat(FloatTest.OpCode, FloatTest.Mnemonic, () => new FloatTest());
            AddFloat(Floor.OpCode, Floor.Mnemonic, () => new Floor());
            AddFloat(PopFloat.OpCode, PopFloat.Mnemonic, () => new PopFloat());
            AddFloat(PushFloat.OpCode, PushFloat.Mnemonic, () => new PushFloat());
            AddFloat(Round.OpCode, Round.Mnemonic, () => new Round());

            // F3 Instructions

            AddFloat(FloatAdd.OpCode, FloatAdd.Mnemonic, () => new FloatAdd());
            AddFloat(FloatAddWithCarry.OpCode, FloatAddWithCarry.Mnemonic, () => new FloatAddWithCarry());
            AddFloat(FloatDivide.OpCode, FloatDivide.Mnemonic, () => new FloatDivide());
            AddFloat(FloatMultiply.OpCode, FloatMultiply.Mnemonic, () => new FloatMultiply());
            AddFloat(FloatSubtract.OpCode, FloatSubtract.Mnemonic, () => new FloatSubtract());
            AddFloat(FloatSubtractWithBorrow.OpCode, FloatSubtractWithBorrow.Mnemonic, () => new FloatSubtractWithBorrow());

            // Fm Instructions

            AddFloat(FloatAddImmediate.OpCode, FloatAddImmediate.Mnemonic, () => new FloatAddImmediate());
            AddFloat(FloatAddImmediateWithCarry.OpCode, FloatAddImmediateWithCarry.Mnemonic, () => new FloatAddImmediateWithCarry());
            AddFloat(FloatAndImmediate.OpCode, FloatAndImmediate.Mnemonic, () => new FloatAndImmediate());
            AddFloat(null, FloatJumpAboveEqual.Mnemonic, () => new FloatJumpAboveEqual());
            AddFloat(FloatJumpAboveThan.OpCode, FloatJumpAboveThan.Mnemonic, () => new FloatJumpAboveThan());
            AddFloat(FloatJumpBelowEqual.OpCode, FloatJumpBelowEqual.Mnemonic, () => new FloatJumpBelowEqual());
            AddFloat(null, FloatJumpBelowThan.Mnemonic, () => new FloatJumpBelowThan());
            AddFloat(null, FloatJumpEqual.Mnemonic, () => new FloatJumpEqual());
            AddFloat(FloatJumpGreaterEqual.OpCode, FloatJumpGreaterEqual.Mnemonic, () => new FloatJumpGreaterEqual());
            AddFloat(FloatJumpGreaterThan.OpCode, FloatJumpGreaterThan.Mnemonic, () => new FloatJumpGreaterThan());
            AddFloat(FloatJumpLessEqual.OpCode, FloatJumpLessEqual.Mnemonic, () => new FloatJumpLessEqual());
            AddFloat(FloatJumpLessThan.OpCode, FloatJumpLessThan.Mnemonic, () => new FloatJumpLessThan());
            AddFloat(FloatJumpNoCarry.OpCode, FloatJumpNoCarry.Mnemonic, () => new FloatJumpNoCarry());
            AddFloat(FloatJumpNoOverflow.OpCode, FloatJumpNoOverflow.Mnemonic, () => new FloatJumpNoOverflow());
            AddFloat(FloatJumpNoParity.OpCode, FloatJumpNoParity.Mnemonic, () => new FloatJumpNoParity());
            AddFloat(FloatJumpNoSign.OpCode, FloatJumpNoSign.Mnemonic, () => new FloatJumpNoSign());
            AddFloat(null, FloatJumpNotEqual.Mnemonic, () => new FloatJumpNotEqual());
            AddFloat(FloatJumpNoZero.OpCode, FloatJumpNoZero.Mnemonic, () => new FloatJumpNoZero());
            AddFloat(FloatJumpYesCarry.OpCode, FloatJumpYesCarry.Mnemonic, () => new FloatJumpYesCarry());
            AddFloat(FloatJumpYesOverflow.OpCode, FloatJumpYesOverflow.Mnemonic, () => new FloatJumpYesOverflow());
            AddFloat(FloatJumpYesParity.OpCode, FloatJumpYesParity.Mnemonic, () => new FloatJumpYesParity());
            AddFloat(FloatJumpYesSign.OpCode, FloatJumpYesSign.Mnemonic, () => new FloatJumpYesSign());
            AddFloat(FloatJumpYesZero.OpCode, FloatJumpYesZero.Mnemonic, () => new FloatJumpYesZero());
            AddFloat(FloatOrImmediate.OpCode, FloatOrImmediate.Mnemonic, () => new FloatOrImmediate());
            AddFloat(FloatSubtractImmediate.OpCode, FloatSubtractImmediate.Mnemonic, () => new FloatSubtractImmediate());
            AddFloat(FloatSubtractImmediateWithBorrow.OpCode, FloatSubtractImmediateWithBorrow.Mnemonic, () => new FloatSubtractImmediateWithBorrow());
            AddFloat(FloatXorImmediate.OpCode, FloatXorImmediate.Mnemonic, () => new FloatXorImmediate());
            AddFloat(LoadFloat.OpCode, LoadFloat.Mnemonic, () => new LoadFloat());
            AddFloat(StoreFloat.OpCode, StoreFloat.Mnemonic, () => new StoreFloat());

            // R2 Instructions

            Add(AbsoluteValue.OpCode, AbsoluteValue.Mnemonic, () => new AbsoluteValue());
            Add(BitScanForward.OpCode, BitScanForward.Mnemonic, () => new BitScanForward());
            Add(BitScanReverse.OpCode, BitScanReverse.Mnemonic, () => new BitScanReverse());
            Add(BitwiseNot.OpCode, BitwiseNot.Mnemonic, () => new BitwiseNot());
            Add(ClearCarryFlag.OpCode, ClearCarryFlag.Mnemonic, () => new ClearCarryFlag());
            Add(ClearOverflowFlag.OpCode, ClearOverflowFlag.Mnemonic, () => new ClearOverflowFlag());
            Add(ClearParityFlag.OpCode, ClearParityFlag.Mnemonic, () => new ClearParityFlag());
            Add(ClearSignFlag.OpCode, ClearSignFlag.Mnemonic, () => new ClearSignFlag());
            Add(ClearZeroFlag.OpCode, ClearZeroFlag.Mnemonic, () => new ClearZeroFlag());
            Add(Compare.OpCode, Compare.Mnemonic, () => new Compare());
            Add(null, CopyRegToReg.Mnemonic, () => new CopyRegToReg());
            Add(ExchangeRegisters.OpCode, ExchangeRegisters.Mnemonic, () => new ExchangeRegisters());
            Add(ExtendLowByte.OpCode, ExtendLowByte.Mnemonic, () => new ExtendLowByte());
            Add(Halt.OpCode, Halt.Mnemonic, () => new Halt());
            Add(LoadFlagsLow.OpCode, LoadFlagsLow.Mnemonic, () => new LoadFlagsLow());
            Add(null, MoveAboveEqual.Mnemonic, () => new MoveAboveEqual());
            Add(MoveAboveThan.OpCode, MoveAboveThan.Mnemonic, () => new MoveAboveThan());
            Add(MoveBelowEqual.OpCode, MoveBelowEqual.Mnemonic, () => new MoveBelowEqual());
            Add(null, MoveBelowThan.Mnemonic, () => new MoveBelowThan());
            Add(null, MoveEqual.Mnemonic, () => new MoveEqual());
            Add(MoveGreaterEqual.OpCode, MoveGreaterEqual.Mnemonic, () => new MoveGreaterEqual());
            Add(MoveGreaterThan.OpCode, MoveGreaterThan.Mnemonic, () => new MoveGreaterThan());
            Add(MoveLessEqual.OpCode, MoveLessEqual.Mnemonic, () => new MoveLessEqual());
            Add(MoveLessThan.OpCode, MoveLessThan.Mnemonic, () => new MoveLessThan());
            Add(MoveNoCarry.OpCode, MoveNoCarry.Mnemonic, () => new MoveNoCarry());
            Add(MoveNoOverflow.OpCode, MoveNoOverflow.Mnemonic, () => new MoveNoOverflow());
            Add(MoveNoParity.OpCode, MoveNoParity.Mnemonic, () => new MoveNoParity());
            Add(MoveNoSign.OpCode, MoveNoSign.Mnemonic, () => new MoveNoSign());
            Add(null, MoveNotEqual.Mnemonic, () => new MoveNotEqual());
            Add(MoveNoZero.OpCode, MoveNoZero.Mnemonic, () => new MoveNoZero());
            Add(MoveUnconditional.OpCode, MoveUnconditional.Mnemonic, () => new MoveUnconditional());
            Add(MoveYesCarry.OpCode, MoveYesCarry.Mnemonic, () => new MoveYesCarry());
            Add(MoveYesOverflow.OpCode, MoveYesOverflow.Mnemonic, () => new MoveYesOverflow());
            Add(MoveYesParity.OpCode, MoveYesParity.Mnemonic, () => new MoveYesParity());
            Add(MoveYesSign.OpCode, MoveYesSign.Mnemonic, () => new MoveYesSign());
            Add(MoveYesZero.OpCode, MoveYesZero.Mnemonic, () => new MoveYesZero());
            Add(Negate.OpCode, Negate.Mnemonic, () => new Negate());
            Add(NoOperation.OpCode, NoOperation.Mnemonic, () => new NoOperation());
            Add(PopHighByte.OpCode, PopHighByte.Mnemonic, () => new PopHighByte());
            Add(PopLowByte.OpCode, PopLowByte.Mnemonic, () => new PopLowByte());
            Add(PopWord.OpCode, PopWord.Mnemonic, () => new PopWord());
            Add(PushHighByte.OpCode, PushHighByte.Mnemonic, () => new PushHighByte());
            Add(PushLowByte.OpCode, PushLowByte.Mnemonic, () => new PushLowByte());
            Add(PushWord.OpCode, PushWord.Mnemonic, () => new PushWord());
            Add(Return.OpCode, Return.Mnemonic, () => new Return());
            Add(SetCarryFlag.OpCode, SetCarryFlag.Mnemonic, () => new SetCarryFlag());
            Add(SetOverflowFlag.OpCode, SetOverflowFlag.Mnemonic, () => new SetOverflowFlag());
            Add(SetParityFlag.OpCode, SetParityFlag.Mnemonic, () => new SetParityFlag());
            Add(SetSignFlag.OpCode, SetSignFlag.Mnemonic, () => new SetSignFlag());
            Add(SetZeroFlag.OpCode, SetZeroFlag.Mnemonic, () => new SetZeroFlag());
            Add(StoreFlagsLow.OpCode, StoreFlagsLow.Mnemonic, () => new StoreFlagsLow());
            Add(Test.OpCode, Test.Mnemonic, () => new Test());

            // R3 Instructions

            Add(Instructions.R3Types.Add.OpCode, Instructions.R3Types.Add.Mnemonic, () => new Add());
            Add(AddWithCarry.OpCode, AddWithCarry.Mnemonic, () => new AddWithCarry());
            Add(ArithmeticShiftLeft.OpCode, ArithmeticShiftLeft.Mnemonic, () => new ArithmeticShiftLeft());
            Add(ArithmeticShiftRight.OpCode, ArithmeticShiftRight.Mnemonic, () => new ArithmeticShiftRight());
            Add(BitwiseAnd.OpCode, BitwiseAnd.Mnemonic, () => new BitwiseAnd());
            Add(BitwiseOr.OpCode, BitwiseOr.Mnemonic, () => new BitwiseOr());
            Add(BitwiseXor.OpCode, BitwiseXor.Mnemonic, () => new BitwiseXor());
            Add(Divide.OpCode, Divide.Mnemonic, () => new Divide());
            Add(null, LogicShiftLeft.Mnemonic, () => new LogicShiftLeft());
            Add(LogicShiftRight.OpCode, LogicShiftRight.Mnemonic, () => new LogicShiftRight());
            Add(Multiply.OpCode, Multiply.Mnemonic, () => new Multiply());
            Add(RotateLeft.OpCode, RotateLeft.Mnemonic, () => new RotateLeft());
            Add(RotateLeftWithCarry.OpCode, RotateLeftWithCarry.Mnemonic, () => new RotateLeftWithCarry());
            Add(RotateRight.OpCode, RotateRight.Mnemonic, () => new RotateRight());
            Add(RotateRightWithCarry.OpCode, RotateRightWithCarry.Mnemonic, () => new RotateRightWithCarry());
            Add(Subtract.OpCode, Subtract.Mnemonic, () => new Subtract());
            Add(SubtractWithBorrow.OpCode, SubtractWithBorrow.Mnemonic, () => new SubtractWithBorrow());

            // Rm Instructions

            Add(AddImmediate.OpCode, AddImmediate.Mnemonic, () => new AddImmediate());
            Add(AddImmediateWithCarry.OpCode, AddImmediateWithCarry.Mnemonic, () => new AddImmediateWithCarry());
            Add(AndImmediate.OpCode, AndImmediate.Mnemonic, () => new AndImmediate());
            Add(Call.OpCode, Call.Mnemonic, () => new Call());
            Add(null, JumpAboveEqual.Mnemonic, () => new JumpAboveEqual());
            Add(JumpAboveThan.OpCode, JumpAboveThan.Mnemonic, () => new JumpAboveThan());
            Add(JumpBelowEqual.OpCode, JumpBelowEqual.Mnemonic, () => new JumpBelowEqual());
            Add(null, JumpBelowThan.Mnemonic, () => new JumpBelowThan());
            Add(null, JumpEqual.Mnemonic, () => new JumpEqual());
            Add(JumpGreaterEqual.OpCode, JumpGreaterEqual.Mnemonic, () => new JumpGreaterEqual());
            Add(JumpGreaterThan.OpCode, JumpGreaterThan.Mnemonic, () => new JumpGreaterThan());
            Add(JumpLessEqual.OpCode, JumpLessEqual.Mnemonic, () => new JumpLessEqual());
            Add(JumpLessThan.OpCode, JumpLessThan.Mnemonic, () => new JumpLessThan());
            Add(JumpNoCarry.OpCode, JumpNoCarry.Mnemonic, () => new JumpNoCarry());
            Add(JumpNoOverflow.OpCode, JumpNoOverflow.Mnemonic, () => new JumpNoOverflow());
            Add(JumpNoParity.OpCode, JumpNoParity.Mnemonic, () => new JumpNoParity());
            Add(JumpNoSign.OpCode, JumpNoSign.Mnemonic, () => new JumpNoSign());
            Add(null, JumpNotEqual.Mnemonic, () => new JumpNotEqual());
            Add(JumpNoZero.OpCode, JumpNoZero.Mnemonic, () => new JumpNoZero());
            Add(JumpUnconditional.OpCode, JumpUnconditional.Mnemonic, () => new JumpUnconditional());
            Add(JumpYesCarry.OpCode, JumpYesCarry.Mnemonic, () => new JumpYesCarry());
            Add(JumpYesOverflow.OpCode, JumpYesOverflow.Mnemonic, () => new JumpYesOverflow());
            Add(JumpYesParity.OpCode, JumpYesParity.Mnemonic, () => new JumpYesParity());
            Add(JumpYesSign.OpCode, JumpYesSign.Mnemonic, () => new JumpYesSign());
            Add(JumpYesZero.OpCode, JumpYesZero.Mnemonic, () => new JumpYesZero());
            Add(LoadEffectiveAddress.OpCode, LoadEffectiveAddress.Mnemonic, () => new LoadEffectiveAddress());
            Add(LoadHighByte.OpCode, LoadHighByte.Mnemonic, () => new LoadHighByte());
            Add(LoadLowByte.OpCode, LoadLowByte.Mnemonic, () => new LoadLowByte());
            Add(LoadWord.OpCode, LoadWord.Mnemonic, () => new LoadWord());
            Add(OrImmediate.OpCode, OrImmediate.Mnemonic, () => new OrImmediate());
            Add(SetMemoryBasePointer.OpCode, SetMemoryBasePointer.Mnemonic, () => new SetMemoryBasePointer());
            Add(StoreHighByte.OpCode, StoreHighByte.Mnemonic, () => new StoreHighByte());
            Add(StoreLowByte.OpCode, StoreLowByte.Mnemonic, () => new StoreLowByte());
            Add(StoreWord.OpCode, StoreWord.Mnemonic, () => new StoreWord());
            Add(SubtractImmediate.OpCode, SubtractImmediate.Mnemonic, () => new SubtractImmediate());
            Add(SubtractImmediateWithBorrow.OpCode, SubtractImmediateWithBorrow.Mnemonic, () => new SubtractImmediateWithBorrow());
            Add(XorImmediate.OpCode, XorImmediate.Mnemonic, () => new XorImmediate());

            // Rs Instructions

            Add(Decrement.OpCode, Decrement.Mnemonic, () => new Decrement());
            Add(Increment.OpCode, Increment.Mnemonic, () => new Increment());
            Add(RotateLeftImmediate.OpCode, RotateLeftImmediate.Mnemonic, () => new RotateLeftImmediate());
            Add(RotateLeftImmediateWithCarry.OpCode, RotateLeftImmediateWithCarry.Mnemonic, () => new RotateLeftImmediateWithCarry());
            Add(RotateRightImmediate.OpCode, RotateRightImmediate.Mnemonic, () => new RotateRightImmediate());
            Add(RotateRightImmediateWithCarry.OpCode, RotateRightImmediateWithCarry.Mnemonic, () => new RotateRightImmediateWithCarry());
            Add(ShiftArithmeticallyLeft.OpCode, ShiftArithmeticallyLeft.Mnemonic, () => new ShiftArithmeticallyLeft());
            Add(ShiftArithmeticallyRight.OpCode, ShiftArithmeticallyRight.Mnemonic, () => new ShiftArithmeticallyRight());
            Add(null, ShiftLogicallyLeft.Mnemonic, () => new ShiftLogicallyLeft());
            Add(ShiftLogicallyRight.OpCode, ShiftLogicallyRight.Mnemonic, () => new ShiftLogicallyRight());
        }

        public void Add(ushort? opcode, string mnemonic, Func<IInstruction> constructor)
        {
            // Op Code will be null if Mnemonic is an alias for another instruction
            if (opcode != null)
                opcodeDictionary[(ushort)opcode] = constructor;
            normalMnemonicDictionary[mnemonic] = constructor;
        }

        public void AddFloat(ushort? opcode, string mnemonic, Func<IInstruction> constructor)
        {
            // Op Code will be null if Mnemonic is an alias for another instruction
            if (opcode != null)
                opcodeDictionary[(ushort) opcode] = constructor;
            floatMnemonicDictionary[mnemonic] = constructor;
        }

        public IInstruction Get(ushort opcode)
        {
            var constructor = opcodeDictionary[opcode];
            return constructor();
        }

        public IInstruction Get(string mnemonic)
        {
            var constructor = normalMnemonicDictionary[mnemonic];
            return constructor();
        }

        public IInstruction GetFloat(string mnemonic)
        {
            var constructor = floatMnemonicDictionary[mnemonic];
            return constructor();
        }
    }
}
