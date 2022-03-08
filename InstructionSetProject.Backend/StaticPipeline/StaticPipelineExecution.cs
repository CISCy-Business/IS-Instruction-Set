using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.Instructions.F2Types;
using InstructionSetProject.Backend.Instructions.FmTypes;
using InstructionSetProject.Backend.Instructions.R2Types;
using InstructionSetProject.Backend.Instructions.RmTypes;
using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend.StaticPipeline
{
    public class StaticPipelineExecution : IExecution
    {
        public InstructionList InstrList;
        public StaticPipelineDataStructures DataStructures;
        public List<byte> MachineCode;
        public StaticPipelineStatistics Statistics;

        public Alu Alu;

        public DecodeExecute DecodeExecute;
        public ExecuteMemory ExecuteMemory;
        public MemoryWriteBack MemoryWriteBack;

        private int _fetchStageOffset = 0;
        private int _decodeStageOffset = -1;
        private int _executeStageOffset = -1;
        private int _memoryStageOffset = -1;
        private int _writeBackStageOffset = -1;

        public IInstruction? fetchingInstruction => (_fetchStageOffset >= 0 && _fetchStageOffset <= InstrList.MaxOffset) ? InstrList.GetFromOffset(_fetchStageOffset) : null;
        public IInstruction? decodingInstruction => (_decodeStageOffset >= 0 && _decodeStageOffset <= InstrList.MaxOffset) ? InstrList.GetFromOffset(_decodeStageOffset) : null;
        public IInstruction? executingInstruction => (_executeStageOffset >= 0 && _executeStageOffset <= InstrList.MaxOffset) ? InstrList.GetFromOffset(_executeStageOffset) : null;
        public IInstruction? memoryInstruction => (_memoryStageOffset >= 0 && _memoryStageOffset <= InstrList.MaxOffset) ? InstrList.GetFromOffset(_memoryStageOffset) : null;
        public IInstruction? writingBackInstruction => (_writeBackStageOffset >= 0 && _writeBackStageOffset <= InstrList.MaxOffset) ? InstrList.GetFromOffset(_writeBackStageOffset) : null;

        public IInstruction? nextInstructionToFinish => writingBackInstruction ?? (memoryInstruction ?? (executingInstruction ?? (decodingInstruction ?? (fetchingInstruction ?? null))));

        public StaticPipelineExecution(InstructionList instrList)
        {
            DataStructures = new();
            Alu = new(DataStructures);
            InstrList = instrList;
            MachineCode = Assembler.Assemble(instrList);
            DataStructures.Memory.AddInstructionCode(MachineCode);
            Statistics = new();
        }

        public void Continue()
        {
            while (fetchingInstruction != null || decodingInstruction != null || executingInstruction != null || memoryInstruction != null || writingBackInstruction != null)
                Step();
        }

        public void Step()
        {
            while (writingBackInstruction != nextInstructionToFinish)
            {
                ClockTick();
            }
            ClockTick();
        }

        public void ClockTick()
        {
            WriteBack();
            MemoryAccess();
            Execute();
            Decode();
            Fetch();

            _writeBackStageOffset = _memoryStageOffset;
            _memoryStageOffset = _executeStageOffset;
            _executeStageOffset = _decodeStageOffset;
            _decodeStageOffset = _fetchStageOffset;
            if (_fetchStageOffset != DataStructures.InstructionPointer.value)
            {
                _fetchStageOffset = DataStructures.InstructionPointer.value;
            }
            else
            {
                _fetchStageOffset = -1;
            }
            Statistics.ClockTicks++;
        }

        private void Fetch()
        {
            var instr = fetchingInstruction;
            if (instr == null) return;

            DataStructures.InstructionPointer.value += instr.lengthInBytes;
        }

        private void Decode()
        {
            var instr = decodingInstruction;
            if (instr == null)
            {
                DecodeExecute.Immediate = null;
                DecodeExecute.ReadData1 = null;
                DecodeExecute.ReadData2 = null;
                DecodeExecute.WriteRegister = null;
                return;
            }

            if (instr is IImmediateInstruction immediateInstr)
            {
                DecodeExecute.Immediate = immediateInstr.GenerateImmediate();
            }
            else
            {
                DecodeExecute.Immediate = null;
            }

            var readReg1 = GetFirstReadRegister(instr);
            if (readReg1 != null)
                DecodeExecute.ReadData1 = readReg1.value;
            else
                DecodeExecute.ReadData1 = null;

            var readReg2 = GetSecondReadRegister(instr);
            if (readReg2 != null)
                DecodeExecute.ReadData2 = readReg2.value;
            else
                DecodeExecute.ReadData2 = null;

            if (instr is StoreWord || instr is StoreFloat || instr is PushWord || instr is PushFloat)
                (DecodeExecute.ReadData1, DecodeExecute.ReadData2) = (DecodeExecute.ReadData2, DecodeExecute.ReadData1);

            DecodeExecute.WriteRegister = GetDestinationRegister(instr);
        }

        private void Execute()
        {
            var instr = executingInstruction;
            if (instr == null)
            {
                ExecuteMemory.ReadData2 = null;
                ExecuteMemory.WriteRegister = null;
                ExecuteMemory.AluResult = null;
                ExecuteMemory.FlagsRegister = null;
                return;
            }

            ExecuteMemory.WriteRegister = DecodeExecute.WriteRegister;

            ushort? aluSource2 = instr.controlBits.ALUSrc ? DecodeExecute.Immediate : DecodeExecute.ReadData2;
            var aluOutput = Alu.Execute((AluOperation)instr.aluOperation, DecodeExecute.ReadData1, aluSource2);
            ExecuteMemory.AluResult = aluOutput.result;
            ExecuteMemory.FlagsRegister = aluOutput.flags;
            if (instr.controlBits.UpdateFlags)
                DataStructures.Flags = aluOutput.flags;

            ExecuteMemory.ReadData2 = DecodeExecute.ReadData2;

            ExecuteMemory.Immediate = DecodeExecute.Immediate;
        }

        private void MemoryAccess()
        {
            var instr = memoryInstruction;
            if (instr == null)
            {
                MemoryWriteBack.WriteRegister = null;
                MemoryWriteBack.AluResult = null;
                MemoryWriteBack.ReadData = null;
                return;
            }

            MemoryWriteBack.WriteRegister = ExecuteMemory.WriteRegister;

            MemoryWriteBack.AluResult = ExecuteMemory.AluResult;

            if (instr.controlBits.MemRead)
            {
                MemoryWriteBack.ReadData = PerformMemRead(instr);
            }
            else if (instr.controlBits.MemWrite)
            {
                PerformMemWrite(instr);
                MemoryWriteBack.ReadData = null;
            }
            else
            {
                MemoryWriteBack.ReadData = null;
            }

            if (instr.controlBits.PCSrc && ExecuteMemory.Immediate != null)
            {
                if (instr is IFlagInstruction flagInstr)
                {
                    if (ExecuteMemory.FlagsRegister != null && ExecuteMemory.FlagsRegister?.IsFlagSet(flagInstr.flagToCheck) == flagInstr.flagEnabled)
                    {
                        DataStructures.InstructionPointer.value = (ushort)ExecuteMemory.Immediate;
                        FlushPipeline();
                    }
                }
                else
                {
                    DataStructures.InstructionPointer.value = (ushort)ExecuteMemory.Immediate;
                    FlushPipeline();
                }
            }
        }

        private void WriteBack()
        {
            var instr = writingBackInstruction;
            if (instr == null) return;

            if (instr.controlBits.RegWrite && MemoryWriteBack.WriteRegister != DataStructures.R0)
            {
                if (MemoryWriteBack.WriteRegister == null)
                {
                    throw new Exception("Attempt to write to null register");
                }

                if (instr.controlBits.MemToReg)
                {
                    if (MemoryWriteBack.ReadData == null)
                    {
                        throw new Exception("Attempt to write null value to register");
                    }

                    MemoryWriteBack.WriteRegister.value = (ushort)MemoryWriteBack.ReadData;
                }
                else
                {
                    if (MemoryWriteBack.AluResult == null)
                    {
                        throw new Exception("Attempt to write null value to register");
                    }

                    MemoryWriteBack.WriteRegister.value = (ushort)MemoryWriteBack.AluResult;
                }
            }
            Statistics.StatInstructionType(writingBackInstruction);
        }

        private ushort PerformMemRead(IInstruction instr)
        {
            if (instr is LoadWord || instr is LoadFloat)
            {
                if (ExecuteMemory.AluResult == null || instr.addressingMode == null)
                    throw new Exception("Null read values");
                return DataStructures.Memory.ReadUshort(ExecuteMemory.AluResult ?? 0, instr.addressingMode ?? 0);
            }

            if (instr is PopWord || instr is PopFloat)
            {
                return DataStructures.Memory.StackPopWord();
            }

            throw new Exception("Unsupported read instruction");
        }

        private void PerformMemWrite(IInstruction instr)
        {
            if (instr is StoreWord || instr is StoreFloat)
            {
                if (ExecuteMemory.AluResult == null || instr.addressingMode == null || ExecuteMemory.ReadData2 == null)
                    throw new Exception("Null write values");
                DataStructures.Memory.WriteUshort(ExecuteMemory.AluResult ?? 0, ExecuteMemory.ReadData2 ?? 0, instr.addressingMode ?? 0);
                return;
            }

            if (instr is PushWord || instr is PushFloat)
            {
                DataStructures.Memory.StackPushWord(ExecuteMemory.ReadData2 ?? 0);
                return;
            }

            throw new Exception("Unsupported write instruction");
        }

        private void FlushPipeline()
        {
            _executeStageOffset = -1;
            _decodeStageOffset = -1;
            _fetchStageOffset = -1;
            Statistics.FlushCount++;
        }

        private Register<ushort>? GetDestinationRegister(IInstruction instr)
        {
            if (instr.firstRegisterType == RegisterType.Write)
                return GetFirstRegister(instr);
            return null;
        }

        private Register<ushort>? GetFirstReadRegister(IInstruction instr)
        {
            if (instr.firstRegisterType == RegisterType.Read)
                return GetFirstRegister(instr);
            if (instr.secondRegisterType == RegisterType.Read)
                return GetSecondRegister(instr);
            if (instr is IImmediateRegister &&
                (instr.addressingMode == 0b001_0000 || instr.addressingMode == 0b001_1000))
                return GetImmediateRegister(instr);
            return null;
        }

        private Register<ushort>? GetSecondReadRegister(IInstruction instr)
        {
            if (instr.firstRegisterType == RegisterType.Read && instr.secondRegisterType == RegisterType.Read)
                return GetSecondRegister(instr);
            if (instr.thirdRegisterType == RegisterType.Read)
                return GetThirdRegister(instr);
            if (instr is IImmediateRegister && instr.firstRegisterType == RegisterType.Read &&
                (instr.addressingMode == 0b001_0000 || instr.addressingMode == 0b001_1000))
                return GetImmediateRegister(instr);
            return null;
        }

        private Register<ushort>? GetFirstRegister(IInstruction instr)
        {
            if (instr is F2Instruction f2Instr)
                return ConvertRegisterIndexToFloatRegister(f2Instr.firstRegister ?? 0);
            if (instr is R2Instruction r2Instr)
                return ConvertRegisterIndexToIntRegister(r2Instr.firstRegister ?? 0);
            if (instr is F3Instruction f3Instr)
                return ConvertRegisterIndexToFloatRegister(f3Instr.firstRegister ?? 0);
            if (instr is R3Instruction r3Instr)
                return ConvertRegisterIndexToIntRegister(r3Instr.firstRegister ?? 0);
            if (instr is RsInstruction rsInstr)
                return ConvertRegisterIndexToIntRegister(rsInstr.firstRegister ?? 0);
            if (instr is RmInstruction rmInstr)
                return ConvertRegisterIndexToIntRegister(rmInstr.firstRegister ?? 0);
            if (instr is FmInstruction fmInstr)
                return ConvertRegisterIndexToFloatRegister(fmInstr.firstRegister ?? 0);
            return null;
        }

        private Register<ushort>? GetSecondRegister(IInstruction instr)
        {
            if (instr is F2Instruction f2Instr)
                return ConvertRegisterIndexToIntRegister((ushort)((f2Instr.secondRegister ?? 0) >> 3));
            if (instr is R2Instruction r2Instr)
                return ConvertRegisterIndexToFloatRegister((ushort)((r2Instr.secondRegister ?? 0) >> 3));
            if (instr is F3Instruction f3Instr)
                return ConvertRegisterIndexToIntRegister((ushort)((f3Instr.secondRegister ?? 0) >> 3));
            if (instr is R3Instruction r3Instr)
                return ConvertRegisterIndexToFloatRegister((ushort)((r3Instr.secondRegister ?? 0) >> 3));
            if (instr is RsInstruction rsInstr)
                return ConvertRegisterIndexToIntRegister((ushort)((rsInstr.secondRegister ?? 0) >> 3));
            if (instr is RmInstruction rmInstr)
                return ConvertRegisterIndexToIntRegister((ushort)((rmInstr.secondRegister ?? 0) >> 3));
            if (instr is FmInstruction fmInstr)
                return ConvertRegisterIndexToFloatRegister((ushort)((fmInstr.secondRegister ?? 0) >> 3));
            return null;
        }

        private Register<ushort>? GetThirdRegister(IInstruction instr)
        {
            if (instr is R3Instruction r3Instr)
                return ConvertRegisterIndexToIntRegister((ushort)((r3Instr.thirdRegister ?? 0) >> 6));
            if (instr is F3Instruction f3Instr)
                return ConvertRegisterIndexToFloatRegister((ushort)((f3Instr.thirdRegister ?? 0) >> 6));
            return null;
        }

        private Register<ushort>? GetImmediateRegister(IInstruction instr)
        {
            if (instr is RmInstruction rmInstr)
                return ConvertRegisterIndexToIntRegister(rmInstr.immediate ?? 0);
            if (instr is FmInstruction fmInstr)
                return ConvertRegisterIndexToFloatRegister(fmInstr.immediate ?? 0);
            return null;
        }

        private Register<ushort> ConvertRegisterIndexToIntRegister(ushort index)
        {
            switch (index)
            {
                case 1:
                    return DataStructures.R1;
                case 2:
                    return DataStructures.R2;
                case 3:
                    return DataStructures.R3;
                case 4:
                    return DataStructures.R4;
                case 5:
                    return DataStructures.R5;
                case 6:
                    return DataStructures.R6;
                case 7:
                    return DataStructures.R7;
                case 8:
                    return DataStructures.MemoryBasePointer;
                default:
                    return DataStructures.R0;
            }
        }

        private Register<ushort> ConvertRegisterIndexToFloatRegister(ushort index)
        {
            switch (index)
            {
                case 1:
                    return DataStructures.F1;
                case 2:
                    return DataStructures.F2;
                case 3:
                    return DataStructures.F3;
                case 4:
                    return DataStructures.F4;
                case 5:
                    return DataStructures.F5;
                case 6:
                    return DataStructures.F6;
                case 7:
                    return DataStructures.F7;
                default:
                    return DataStructures.F0;
            }
        }
    }

    public struct DecodeExecute
    {
        public ushort? ReadData1;
        public ushort? ReadData2;
        public ushort? Immediate;
        public Register<ushort>? WriteRegister;
    }

    public struct ExecuteMemory
    {
        public ushort? AluResult;
        public ushort? ReadData2;
        public ushort? Immediate;
        public Register<ushort>? WriteRegister;
        public FlagsRegister? FlagsRegister;
    }

    public struct MemoryWriteBack
    {
        public ushort? ReadData;
        public ushort? AluResult;
        public Register<ushort>? WriteRegister;
    }
}
