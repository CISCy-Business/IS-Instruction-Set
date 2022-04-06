using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.DynamicPipeline
{
    public class DependencyManager
    {
        private PipelineDataStructures dataStructures { get; set; }
        private int?[] integerDependencies { get; } = new int?[8];
        private int?[] floatDependencies { get; } = new int?[8];

        public DependencyManager(PipelineDataStructures dataStructures)
        {
            this.dataStructures = dataStructures;
        }

        public void CheckDependencies(InstructionInFlight instr)
        {
            var writeRegIndex = GetWriteRegister(instr.instruction);
            var readReg1Index = GetFirstReadRegister(instr.instruction);
            var readReg2Index = GetSecondReadRegister(instr.instruction);

            if (!IsInstructionFloat(instr.instruction))
            {
                if (writeRegIndex != null)
                    integerDependencies[(int)writeRegIndex] = instr.Index;
                if (readReg1Index != null && integerDependencies[(int)readReg1Index] != null)
                    instr.lhsDependency = integerDependencies[(int)readReg1Index];
                else if (readReg1Index != null)
                    instr.lhsValue = GetIntegerRegValue((int)readReg1Index);
                if (instr.instruction.controlBits.ALUSrc && (instr.instruction.addressingMode != 0b001_0000 || instr.instruction.addressingMode != 0b0011000))
                {
                    instr.rhsValue = instr.instruction.immediate ?? 0;
                }
                else
                {
                    if (readReg2Index != null && integerDependencies[(int)readReg2Index] != null)
                        instr.rhsDependency = integerDependencies[(int)readReg2Index];
                    else if (readReg2Index != null)
                        instr.rhsValue = GetIntegerRegValue((int)readReg2Index);
                }
            }
            else
            {
                if (writeRegIndex != null)
                    floatDependencies[(int)writeRegIndex] = instr.Index;
                if (readReg1Index != null && floatDependencies[(int)readReg1Index] != null)
                    instr.lhsDependency = floatDependencies[(int)readReg1Index];
                else if (readReg1Index != null)
                    instr.lhsValue = GetFloatRegValue((int)readReg1Index);
                if (instr.instruction.controlBits.ALUSrc && (instr.instruction.addressingMode != 0b001_0000 || instr.instruction.addressingMode != 0b0011000))
                {
                    instr.rhsValue = instr.instruction.immediate ?? 0;
                }
                else
                {
                    if (readReg2Index != null && floatDependencies[(int)readReg2Index] != null)
                        instr.rhsDependency = floatDependencies[(int)readReg2Index];
                    else if (readReg2Index != null)
                        instr.rhsValue = GetFloatRegValue((int)readReg2Index);
                }
            }
        }

        public ushort GetIntegerRegValue(int index)
        {
            switch (index)
            {
                case 0: return dataStructures.R0.value;
                case 1: return dataStructures.R1.value;
                case 2: return dataStructures.R2.value;
                case 3: return dataStructures.R3.value;
                case 4: return dataStructures.R4.value;
                case 5: return dataStructures.R5.value;
                case 6: return dataStructures.R6.value;
                case 7: return dataStructures.R7.value;
                default: throw new Exception();
            }
        }

        public ushort GetFloatRegValue(int index)
        {
            switch (index)
            {
                case 0: return dataStructures.F0.value;
                case 1: return dataStructures.F1.value;
                case 2: return dataStructures.F2.value;
                case 3: return dataStructures.F3.value;
                case 4: return dataStructures.F4.value;
                case 5: return dataStructures.F5.value;
                case 6: return dataStructures.F6.value;
                case 7: return dataStructures.F7.value;
                default: throw new Exception();
            }
        }

        public void UpdateDependency(InstructionInFlight instr)
        {
            if (!IsInstructionFloat(instr.instruction))
            {
                for (var i = 0; i < integerDependencies.Length; i++)
                {
                    if (integerDependencies[i] == instr.Index)
                    {
                        integerDependencies[i] = null;
                        break;
                    }
                }
            }
            else
            {
                for (var i = 0; i < floatDependencies.Length; i++)
                {
                    if (floatDependencies[i] == instr.Index)
                    {
                        floatDependencies[i] = null;
                        break;
                    }
                }

            }
        }

        private int? GetWriteRegister(IInstruction instr)
        {
            if (instr.firstRegisterType == RegisterType.Write)
                return instr.firstRegister;
            return null;
        }

        private int? GetFirstReadRegister(IInstruction instr)
        {
            if (instr.firstRegisterType == RegisterType.Read)
                return instr.firstRegister;
            if (instr.secondRegisterType == RegisterType.Read)
                return instr.secondRegister >> 3;
            if (instr is IImmediateRegister &&
                (instr.addressingMode == 0b001_0000 || instr.addressingMode == 0b0011000))
                return instr.immediate;
            return null;
        }

        private int? GetSecondReadRegister(IInstruction instr)
        {
            if (instr.firstRegisterType == RegisterType.Read && instr.secondRegisterType == RegisterType.Read)
                return instr.secondRegister >> 3;
            if (instr.thirdRegisterType == RegisterType.Read)
                return instr.thirdRegister >> 6;
            if (instr is IImmediateRegister && instr.firstRegisterType == RegisterType.Read &&
                (instr.addressingMode == 0b001_0000 || instr.addressingMode == 0b001_1000))
                return instr.immediate;
            return null;
        }

        private bool IsInstructionFloat(IInstruction instr)
        {
            return instr is F2Instruction or F3Instruction or FmInstruction;
        }
    }
}
