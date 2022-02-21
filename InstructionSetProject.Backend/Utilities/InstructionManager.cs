using InstructionSetProject.Backend.InstructionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Instructions.JumpTypes;
using InstructionSetProject.Backend.Instructions.MemoryTypes;
using InstructionSetProject.Backend.Instructions.R0Types;
using InstructionSetProject.Backend.Instructions.R1Types;
using InstructionSetProject.Backend.Instructions.R2ITypes;
using InstructionSetProject.Backend.Instructions.R2Types;
using InstructionSetProject.Backend.Instructions.R3Types;

namespace InstructionSetProject.Backend.Utilities
{
    public class InstructionManager
    {
        public static InstructionManager Instance = new();

        private Dictionary<ushort, string> keyDictionary = new();
        private Dictionary<string, Func<IInstruction>> valueDictionary = new();

        public InstructionManager()
        {
            // R0 Instructions

            Add(Halt.OpCode, Halt.Mnemonic, () => new Halt());
            Add(NoOperation.OpCode, NoOperation.Mnemonic, () => new NoOperation());

            // R1 Instructions

            Add(PopByteHigh.OpCode, PopByteHigh.Mnemonic, () => new PopByteHigh());
            Add(PopByteLow.OpCode, PopByteLow.Mnemonic, () => new PopByteLow());
            Add(PopWord.OpCode, PopWord.Mnemonic, () => new PopWord());
            Add(PushByteHigh.OpCode, PushByteHigh.Mnemonic, () => new PushByteHigh());
            Add(PushByteLow.OpCode, PushByteLow.Mnemonic, () => new PushByteLow());
            Add(PushWord.OpCode, PushWord.Mnemonic, () => new PushWord());
            Add(SetFlagsExplicit.OpCode, SetFlagsExplicit.Mnemonic, () => new SetFlagsExplicit());
            Add(SetFlagsRegister.OpCode, SetFlagsRegister.Mnemonic, () => new SetFlagsRegister());

            // R2 Instructions

            Add(BitwiseNeg.OpCode, BitwiseNeg.Mnemonic, () => new BitwiseNeg());
            Add(BitwiseNot.OpCode, BitwiseNot.Mnemonic, () => new BitwiseNot());
            Add(Compare.OpCode, Compare.Mnemonic, () => new Compare());
            Add(Exchange.OpCode, Exchange.Mnemonic, () => new Exchange());
            Add(ExchangeAboveEqual.OpCode, ExchangeAboveEqual.Mnemonic, () => new ExchangeAboveEqual());
            Add(ExchangeAboveThan.OpCode, ExchangeAboveThan.Mnemonic, () => new ExchangeAboveThan());
            Add(ExchangeBelowEqual.OpCode, ExchangeBelowEqual.Mnemonic, () => new ExchangeBelowEqual());
            Add(ExchangeBelowThan.OpCode, ExchangeBelowThan.Mnemonic, () => new ExchangeBelowThan());
            Add(ExchangeCarry.OpCode, ExchangeCarry.Mnemonic, () => new ExchangeCarry());
            Add(ExchangeEqual.OpCode, ExchangeEqual.Mnemonic, () => new ExchangeEqual());
            Add(ExchangeGreaterEqual.OpCode, ExchangeGreaterEqual.Mnemonic, () => new ExchangeGreaterEqual());
            Add(ExchangeGreaterThan.OpCode, ExchangeGreaterThan.Mnemonic, () => new ExchangeGreaterThan());
            Add(ExchangeLessEqual.OpCode, ExchangeLessEqual.Mnemonic, () => new ExchangeLessEqual());
            Add(ExchangeLessThan.OpCode, ExchangeLessThan.Mnemonic, () => new ExchangeLessThan());
            Add(ExchangeNotCarry.OpCode, ExchangeNotCarry.Mnemonic, () => new ExchangeNotCarry());
            Add(ExchangeNotEqual.OpCode, ExchangeNotEqual.Mnemonic, () => new ExchangeNotEqual());
            Add(ExchangeNotOverflow.OpCode, ExchangeNotOverflow.Mnemonic, () => new ExchangeNotOverflow());
            Add(ExchangeNotParity.OpCode, ExchangeNotParity.Mnemonic, () => new ExchangeNotParity());
            Add(ExchangeNotSign.OpCode, ExchangeNotSign.Mnemonic, () => new ExchangeNotSign());
            Add(ExchangeNotZero.OpCode, ExchangeNotZero.Mnemonic, () => new ExchangeNotZero());
            Add(ExchangeOverflow.OpCode, ExchangeOverflow.Mnemonic, () => new ExchangeOverflow());
            Add(ExchangeParity.OpCode, ExchangeParity.Mnemonic, () => new ExchangeParity());
            Add(ExchangeSign.OpCode, ExchangeSign.Mnemonic, () => new ExchangeSign());
            Add(ExchangeZero.OpCode, ExchangeZero.Mnemonic, () => new ExchangeZero());
            Add(Test.OpCode, Test.Mnemonic, () => new Test());

            // R2I Instructions

            Add(BitwiseAddImmediate.OpCode, BitwiseAddImmediate.Mnemonic, () => new BitwiseAddImmediate());
            Add(BitwiseAddImmediateCarry.OpCode, BitwiseAddImmediateCarry.Mnemonic, () => new BitwiseAddImmediateCarry());
            Add(BitwiseAndImmediate.OpCode, BitwiseAndImmediate.Mnemonic, () => new BitwiseAndImmediate());
            Add(BitwiseOrImmediate.OpCode, BitwiseOrImmediate.Mnemonic, () => new BitwiseOrImmediate());
            Add(BitwiseSubtractImmediate.OpCode, BitwiseSubtractImmediate.Mnemonic, () => new BitwiseSubtractImmediate());
            Add(BitwiseSubtractImmediateBorrow.OpCode, BitwiseSubtractImmediateBorrow.Mnemonic, () => new BitwiseSubtractImmediateBorrow());
            Add(BitwiseXorImmediate.OpCode, BitwiseXorImmediate.Mnemonic, () => new BitwiseXorImmediate());

            // R3 Instructions

            Add(ArithmeticShiftLeft.OpCode, ArithmeticShiftLeft.Mnemonic, () => new ArithmeticShiftLeft());
            Add(ArithmeticShiftRight.OpCode, ArithmeticShiftRight.Mnemonic, () => new ArithmeticShiftRight());
            Add(BitwiseAdd.OpCode, BitwiseAdd.Mnemonic, () => new BitwiseAdd());
            Add(BitwiseAddCarry.OpCode, BitwiseAddCarry.Mnemonic, () => new BitwiseAddCarry());
            Add(BitwiseAnd.OpCode, BitwiseAnd.Mnemonic, () => new BitwiseAnd());
            Add(BitwiseDivide.OpCode, BitwiseDivide.Mnemonic, () => new BitwiseDivide());
            Add(BitwiseMultiply.OpCode, BitwiseMultiply.Mnemonic, () => new BitwiseMultiply());
            Add(BitwiseOr.OpCode, BitwiseOr.Mnemonic, () => new BitwiseOr());
            Add(BitwiseSubtract.OpCode, BitwiseSubtract.Mnemonic, () => new BitwiseSubtract());
            Add(BitwiseSubtractBorrow.OpCode, BitwiseSubtractBorrow.Mnemonic, () => new BitwiseSubtractBorrow());
            Add(BitwiseXor.OpCode, BitwiseXor.Mnemonic, () => new BitwiseXor());
            Add(LogicalShiftRight.OpCode, LogicalShiftRight.Mnemonic, () => new LogicalShiftRight());
            Add(RotateLeft.OpCode, RotateLeft.Mnemonic, () => new RotateLeft());
            Add(RotateLeftCarry.OpCode, RotateLeftCarry.Mnemonic, () => new RotateLeftCarry());
            Add(RotateRight.OpCode, RotateRight.Mnemonic, () => new RotateRight());
            Add(RotateRightCarry.OpCode, RotateRightCarry.Mnemonic, () => new RotateRightCarry());

            // Memory Instructions

            Add(LoadByteHigh.OpCode, LoadByteHigh.Mnemonic, () => new LoadByteHigh());
            Add(LoadByteLow.OpCode, LoadByteLow.Mnemonic, () => new LoadByteLow());
            Add(LoadWord.OpCode, LoadWord.Mnemonic, () => new LoadWord());
            Add(StoreByteHigh.OpCode, StoreByteHigh.Mnemonic, () => new StoreByteHigh());
            Add(StoreByteLow.OpCode, StoreByteLow.Mnemonic, () => new StoreByteLow());
            Add(StoreWord.OpCode, StoreWord.Mnemonic, () => new StoreWord());

            // Jump Instructions

            Add(JumpAboveEqual.OpCode, JumpAboveEqual.Mnemonic, () => new JumpAboveEqual());
            Add(JumpAboveThan.OpCode, JumpAboveThan.Mnemonic, () => new JumpAboveThan());
            Add(JumpBelowEqual.OpCode, JumpBelowEqual.Mnemonic, () => new JumpBelowEqual());
            Add(JumpBelowThan.OpCode, JumpBelowThan.Mnemonic, () => new JumpBelowThan());
            Add(JumpCarry.OpCode, JumpCarry.Mnemonic, () => new JumpCarry());
            Add(JumpEqual.OpCode, JumpEqual.Mnemonic, () => new JumpEqual());
            Add(JumpGreaterEqual.OpCode, JumpGreaterEqual.Mnemonic, () => new JumpGreaterEqual());
            Add(JumpGreaterThan.OpCode, JumpGreaterThan.Mnemonic, () => new JumpGreaterThan());
            Add(JumpLessEqual.OpCode, JumpLessEqual.Mnemonic, () => new JumpLessEqual());
            Add(JumpLessThan.OpCode, JumpLessThan.Mnemonic, () => new JumpLessThan());
            Add(JumpNotCarry.OpCode, JumpNotCarry.Mnemonic, () => new JumpNotCarry());
            Add(JumpNotEqual.OpCode, JumpNotEqual.Mnemonic, () => new JumpNotEqual());
            Add(JumpNotOverflow.OpCode, JumpNotOverflow.Mnemonic, () => new JumpNotOverflow());
            Add(JumpNotParity.OpCode, JumpNotParity.Mnemonic, () => new JumpNotParity());
            Add(JumpNotSign.OpCode, JumpNotSign.Mnemonic, () => new JumpNotSign());
            Add(JumpNotZero.OpCode, JumpNotZero.Mnemonic, () => new JumpNotZero());
            Add(JumpOverflow.OpCode, JumpOverflow.Mnemonic, () => new JumpOverflow());
            Add(JumpParity.OpCode, JumpParity.Mnemonic, () => new JumpParity());
            Add(JumpSign.OpCode, JumpSign.Mnemonic, () => new JumpSign());
            Add(JumpZero.OpCode, JumpZero.Mnemonic, () => new JumpZero());
        }

        public void Add(ushort opcode, string mnemonic, Func<IInstruction> constructor)
        {
            keyDictionary[opcode] = mnemonic;
            valueDictionary[mnemonic] = constructor;
        }

        public IInstruction Get(ushort opcode)
        {
            var mnemonic = keyDictionary[opcode];
            var constructor = valueDictionary[mnemonic];
            return constructor();
        }

        public IInstruction Get(string mnemonic)
        {
            var constructor = valueDictionary[mnemonic];
            return constructor();
        }
    }
}
