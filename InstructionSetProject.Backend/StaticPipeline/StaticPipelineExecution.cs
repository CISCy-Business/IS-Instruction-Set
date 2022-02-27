using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.StaticPipeline
{
    public class StaticPipelineExecution : IExecution
    {
        public InstructionList InstrList;
        public StaticPipelineDataStructures DataStructures = new();
        public List<byte> MachineCode;

        public FetchDecode FetchDecode = new();
        public DecodeExecute DecodeExecute = new();
        public ExecuteMemory ExecuteMemory = new();
        public MemoryWriteBack MemoryWriteBack = new();

        public int MachineCodeIndex = 0;

        private int _fetchStageOffset = 0;
        private int _decodeStageOffset = -1;
        private int _executeStageOffset = -1;
        private int _memoryStageOffset = -1;
        private int _writeBackStageOffset = -1;

        public IInstruction? fetchingInstruction => (_fetchStageOffset >= 0 && _fetchStageOffset < InstrList.MaxOffset) ? InstrList.GetFromOffset(_fetchStageOffset) : null;
        public IInstruction? decodingInstruction => (_decodeStageOffset >= 0 && _decodeStageOffset < InstrList.MaxOffset) ? InstrList.GetFromOffset(_decodeStageOffset) : null;
        public IInstruction? executingInstruction => (_executeStageOffset >= 0 && _executeStageOffset < InstrList.MaxOffset) ? InstrList.GetFromOffset(_executeStageOffset) : null;
        public IInstruction? memoryInstruction => (_memoryStageOffset >= 0 && _memoryStageOffset < InstrList.MaxOffset) ? InstrList.GetFromOffset(_memoryStageOffset) : null;
        public IInstruction? writingBackInstruction => (_writeBackStageOffset >= 0 && _writeBackStageOffset < InstrList.MaxOffset) ? InstrList.GetFromOffset(_writeBackStageOffset) : null;

        public IInstruction? nextInstructionToFinish => writingBackInstruction ?? (memoryInstruction ?? (executingInstruction ?? (decodingInstruction ?? (fetchingInstruction ?? InstrList.Instructions.FirstOrDefault()))));


        public StaticPipelineExecution(InstructionList instrList)
        {
            InstrList = instrList;
            MachineCode = Assembler.Assemble(instrList);
        }

        public void Continue()
        {
            while (InstrList.GetFromOffset(DataStructures.InstructionPointer.value) != null)
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
        }

        private void Fetch()
        {
            var instr = fetchingInstruction;
            if (instr == null)
            {
                FetchDecode.ProgramCounter = null;
                return;
            }

            FetchDecode.ProgramCounter = DataStructures.InstructionPointer.value;
            if (!instr.functionBits.PCSrc)
                DataStructures.InstructionPointer.value += instr.lengthInBytes;
        }

        private void Decode()
        {
            var instr = decodingInstruction;
            if (instr == null)
            {
                DecodeExecute.Immediate = null;
                DecodeExecute.ProgramCounter = null;
                DecodeExecute.ReadData1 = null;
                DecodeExecute.ReadData2 = null;
                DecodeExecute.WriteRegister = null;
                return;
            }

            DecodeExecute.ProgramCounter = FetchDecode.ProgramCounter;
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
                ExecuteMemory.CalculatedProgramCounter = null;
                return;
            }

            ExecuteMemory.WriteRegister = DecodeExecute.WriteRegister;
            
            ushort? aluSource2 = instr.functionBits.ALUSrc ? DecodeExecute.Immediate : DecodeExecute.ReadData2;
            if (DecodeExecute.ReadData1 == null || aluSource2 == null)
                ExecuteMemory.AluResult = null;
            else
                ExecuteMemory.AluResult = instr.AluOperation((ushort)DecodeExecute.ReadData1, (ushort)aluSource2);

            ExecuteMemory.ReadData2 = DecodeExecute.ReadData2;

            if (instr.functionBits.PCSrc && DecodeExecute.Immediate != null)
            {
                DataStructures.InstructionPointer.value = (ushort)DecodeExecute.Immediate;
            }
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

            if (instr.functionBits.MemRead)
            {
                if (ExecuteMemory.AluResult == null)
                {
                    throw new Exception("Attempt to read from null address");
                }

                MemoryWriteBack.ReadData = DataStructures.Memory.Read((ushort)ExecuteMemory.AluResult);
            }
            else if (instr.functionBits.MemWrite)
            {
                if (ExecuteMemory.AluResult == null)
                {
                    throw new Exception("Attempt to write to null address");
                }

                if (ExecuteMemory.ReadData2 == null)
                {
                    throw new Exception("Attempt to write null value to memory");
                }

                DataStructures.Memory.Write((ushort)ExecuteMemory.AluResult, (ushort)ExecuteMemory.ReadData2);
                MemoryWriteBack.ReadData = null;
            }
            else
            {
                MemoryWriteBack.ReadData = null;
            }
        }

        private void WriteBack()
        {
            var instr = writingBackInstruction;
            if (instr == null) return;

            if (instr.functionBits.RegWrite && MemoryWriteBack.WriteRegister != DataStructures.R0)
            {
                if (MemoryWriteBack.WriteRegister == null)
                {
                    throw new Exception("Attempt to write to null register");
                }

                if (instr.functionBits.MemToReg)
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

                    MemoryWriteBack.WriteRegister.value = (ushort) MemoryWriteBack.AluResult;
                }
            }
        }

        private Register<ushort>? GetFirstReadRegister(IInstruction instr)
        {
            // if (instr is JumpInstruction jumpInstr)
            //     return ConvertRegisterIndexToRegister((ushort) (jumpInstr.SourceRegister >> 3));
            // if (instr is R2Instruction r2Instr)
            //     return ConvertRegisterIndexToRegister((ushort) (r2Instr.SourceRegister >> 3));
            // if (instr is R2IInstruction r2iInstr)
            //     return ConvertRegisterIndexToRegister((ushort) (r2iInstr.SourceRegister >> 3));
            if (instr is R3Instruction r3Instr)
                return ConvertRegisterIndexToRegister((ushort) (r3Instr.SourceRegister1 >> 3));
            return null;
        }

        private Register<ushort>? GetSecondReadRegister(IInstruction instr)
        {
            if (instr is R3Instruction r3Instr)
                return ConvertRegisterIndexToRegister((ushort) (r3Instr.SourceRegister2 >> 6));
            return null;
        }

        private Register<ushort>? GetDestinationRegister(IInstruction instr)
        {
            // if (instr is JumpInstruction jumpInstr)
            //     return ConvertRegisterIndexToRegister(jumpInstr.DestinationRegister);
            // if (instr is MemoryInstruction memInstr)
            //     return ConvertRegisterIndexToRegister(memInstr.DestinationRegister);
            // if (instr is R1Instruction r1Instr)
            //     return ConvertRegisterIndexToRegister(r1Instr.DestinationRegister);
            // if (instr is R2IInstruction r2iInstr)
            //     return ConvertRegisterIndexToRegister(r2iInstr.DestinationRegister);
            // if (instr is R2Instruction r2Instr)
            //     return ConvertRegisterIndexToRegister(r2Instr.DestinationRegister);
            if (instr is R3Instruction r3Instr)
                return ConvertRegisterIndexToRegister(r3Instr.DestinationRegister);
            return null;
        }

        private Register<ushort> ConvertRegisterIndexToRegister(ushort index)
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
                default:
                    return DataStructures.R0;
            }
        }
    }

    public struct FetchDecode
    {
        public ushort? ProgramCounter;
    }

    public struct DecodeExecute
    {
        public ushort? ProgramCounter;
        public ushort? ReadData1;
        public ushort? ReadData2;
        public ushort? Immediate;
        public Register<ushort>? WriteRegister;
    }

    public struct ExecuteMemory
    {
        public ushort? CalculatedProgramCounter;
        public ushort? AluResult;
        public ushort? ReadData2;
        public Register<ushort>? WriteRegister;
    }

    public struct MemoryWriteBack
    {
        public ushort? ReadData;
        public ushort? AluResult;
        public Register<ushort>? WriteRegister;
    }
}
