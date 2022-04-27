using InstructionSetProject.Backend.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.InstructionTypes;

namespace InstructionSetProject.Backend.DynamicPipeline
{
    public class DynamicPipelineExecution
    {
        public InstructionQueue instrQueue { get; set; }
        public MemoryUnit memoryUnit { get; set; }
        public DependencyManager dependencyManager { get; set; }
        public ReorderBuffer reorderBuffer { get; set; }
        public ReservationStation fpAdderReservationStation { get; set; }
        public ReservationStation fpMulReservationStation { get; set; }
        public ReservationStation integerReservationStation { get; set; }
        public PipelineDataStructures dataStructures { get; set; }
        public List<byte> machineCode { get; set; }
        public PipelineStatistics statistics { get; set; }
        public Alu alu { get; set; }
        public InstructionInFlight? fpAdder { get; set; }
        public InstructionInFlight? fpMul { get; set; }
        public InstructionInFlight? integerUnit { get; set; }
        public List<InstructionInFlight> commonDataBus { get; set; }

        public DynamicPipelineExecution(InstructionList instrList, CacheConfiguration l1Config, CacheConfiguration l2Config)
        {
            dataStructures = new(l1Config, l2Config);
            alu = new(dataStructures);
            machineCode = Assembler.Assemble(instrList);
            dataStructures.Memory.AddInstructionCode(machineCode);
            statistics = new();
            dependencyManager = new DependencyManager(dataStructures);
            instrQueue = new InstructionQueue(instrList, dataStructures.InstructionPointer, dependencyManager);
            reorderBuffer = new ReorderBuffer(dataStructures);
            memoryUnit = new MemoryUnit(dataStructures);
            commonDataBus = new();
            fpAdderReservationStation = new();
            fpMulReservationStation = new();
            integerReservationStation = new();
        }

        public void Continue()
        {
            while (!IsExecutionFinished())
                Step();
        }

        public bool IsExecutionFinished()
        {
            return !(
                instrQueue.nextBatch.Count != 0 ||
                memoryUnit.loadBuffers.Count != 0 ||
                memoryUnit.activeInstruction != null ||
                fpAdderReservationStation.instructions.Count != 0 ||
                fpAdder != null ||
                fpMulReservationStation.instructions.Count != 0 ||
                fpMul != null ||
                integerReservationStation.instructions.Count != 0 ||
                integerUnit != null ||
                commonDataBus.Count != 0 ||
                reorderBuffer.buffers.Count != 0 ||
                instrQueue.instructions.InstructionOffsetDictionary.ContainsKey(dataStructures.InstructionPointer.value)
            );
        }

        public void Step()
        {
            while (reorderBuffer.buffers.Count == 0)
                ClockTick();
            ClockTick();
        }

        public void ClockTick()
        {
            reorderBuffer.ClockTick();
            ClockTickCommonDataBus();
            ClockTickUnits();
            ClockTickInstructionQueue();
            statistics.ClockTicks++;
        }

        private void ClockTickCommonDataBus()
        {
            foreach (var instr in commonDataBus)
            {
                dependencyManager.UpdateDependency(instr);
                ResolveDependencies(instr);
                reorderBuffer.buffers.Enqueue(instr, instr.Index);
            }
            commonDataBus.Clear();
        }

        private void ResolveDependencies(InstructionInFlight instr)
        {
            ResolveDependencies(instr, instrQueue.nextBatch);
            ResolveDependencies(instr, memoryUnit.loadBuffers);
            ResolveDependencies(instr, integerReservationStation.instructions);
            ResolveDependencies(instr, fpAdderReservationStation.instructions);
            ResolveDependencies(instr, fpMulReservationStation.instructions);
        }

        private void ResolveDependencies(InstructionInFlight instr, IEnumerable<InstructionInFlight> collection)
        {
            foreach (var collectionInstr in collection)
            {
                if (collectionInstr.lhsDependency == instr.Index)
                    collectionInstr.SetLhsFromDependency(instr.result ?? 0);
                if (collectionInstr.rhsDependency == instr.Index)
                    collectionInstr.SetRhsFromDependency(instr.result ?? 0);
            }
        }

        private void ClockTickUnits()
        {
            var memInstr = memoryUnit.ClockTick();
            if(memInstr != null) commonDataBus.Add(memInstr);
            var fpAddInstr = ClockTickFpAdder();
            if (fpAddInstr != null) commonDataBus.Add(fpAddInstr);
            var fpMulInstr = ClockTickFpMul();
            if (fpMulInstr != null) commonDataBus.Add(fpMulInstr);
            var integerInstr = ClockTickIntegerUnit();
            if (integerInstr != null) commonDataBus.Add(integerInstr);
        }

        private InstructionInFlight? ClockTickFpAdder()
        {
            fpAdder ??= fpAdderReservationStation.GetNextReadyInstruction();

            if (fpAdder == null)
                return null;

            if (fpAdder.cyclesRemainingInExecute > 1)
            {
                fpAdder.cyclesRemainingInExecute--;
                return null;
            }

            var (result, flags) = alu.Execute((AluOperation)fpAdder.instruction.aluOperation, fpAdder.lhsValue, fpAdder.rhsValue);
            fpAdder.result = result;
            var instr = fpAdder;
            fpAdder = null;
            return instr;
        }

        private InstructionInFlight? ClockTickFpMul()
        {
            fpMul ??= fpMulReservationStation.GetNextReadyInstruction();

            if (fpMul == null)
                return null;

            if (fpMul.cyclesRemainingInExecute > 1)
            {
                fpMul.cyclesRemainingInExecute--;
                return null;
            }

            var (result, flags) = alu.Execute((AluOperation)fpMul.instruction.aluOperation, fpMul.lhsValue, fpMul.rhsValue);
            fpMul.result = result;
            var instr = fpMul;
            fpMul = null;
            return instr;
        }

        private InstructionInFlight? ClockTickIntegerUnit()
        {
            integerUnit ??= integerReservationStation.GetNextReadyInstruction();

            if (integerUnit == null)
                return null;

            if (integerUnit.cyclesRemainingInExecute > 1)
            {
                integerUnit.cyclesRemainingInExecute--;
                return null;
            }

            var (result, flags) = alu.Execute((AluOperation)integerUnit.instruction.aluOperation, integerUnit.lhsValue, integerUnit.rhsValue);
            if (integerUnit.instruction.controlBits.PCSrc && integerUnit.instruction.immediate != null)
            {
                if (integerUnit.instruction is IFlagInstruction flagInstr)
                {
                    if (flags.IsFlagSet(flagInstr.flagToCheck) == flagInstr.flagEnabled)
                    {
                        dataStructures.InstructionPointer.value = integerUnit.instruction.immediate ??
                                                                  dataStructures.InstructionPointer.value;
                        FlushPipeline(integerUnit.Index);

                        return null;
                    }
                }
            }
            integerUnit.result = result;
            var instr = integerUnit;
            integerUnit = null;
            return instr;
        }

        private void FlushPipeline(int instrIndex)
        {
            var newReorderBuffer = new PriorityQueue<InstructionInFlight, int>();
            newReorderBuffer.EnqueueRange(reorderBuffer.buffers.UnorderedItems.Where((instr) => instr.Element.Index < instrIndex));
            reorderBuffer.buffers = newReorderBuffer;
            if (fpAdder?.Index >= instrIndex) fpAdder = null;
            if (fpMul?.Index >= instrIndex) fpMul = null;
            if (integerUnit?.Index >= instrIndex) integerUnit = null;
            fpAdderReservationStation.instructions = new Queue<InstructionInFlight>(
                fpAdderReservationStation.instructions.Where((instr) => instr.Index < instrIndex));
            fpMulReservationStation.instructions = new Queue<InstructionInFlight>(
                fpMulReservationStation.instructions.Where((instr) => instr.Index < instrIndex));
            integerReservationStation.instructions = new Queue<InstructionInFlight>(
                integerReservationStation.instructions.Where((instr) => instr.Index < instrIndex));
            if (memoryUnit.activeInstruction?.Index >= instrIndex) memoryUnit.activeInstruction = null;
            memoryUnit.loadBuffers = new Queue<InstructionInFlight>(
                memoryUnit.loadBuffers.Where((instr) => instr.Index < instrIndex));
            instrQueue.nextBatch = instrQueue.nextBatch.Where((instr) => instr.Index < instrIndex).ToList();
            if (instrQueue.instructionIndex > instrIndex) instrQueue.instructionIndex = instrIndex;
        }

        private void ClockTickInstructionQueue()
        {
            List<InstructionInFlight> instructionsHeldForNextBatch = new();
            for (int i = 0; i < instrQueue.nextBatch.Count; i++)
            {
                var instr = instrQueue.nextBatch[i];
                switch (instr.instruction.instructionUnit)
                {
                    case InstructionUnit.Integer:
                        if (integerReservationStation.instructions.Count < 3)
                        {
                            integerReservationStation.instructions.Enqueue(instr);
                        }
                        else
                            instructionsHeldForNextBatch.Add(instr);
                        break;
                    case InstructionUnit.FpAdder:
                        if (fpAdderReservationStation.instructions.Count < 3)
                        {
                            fpAdderReservationStation.instructions.Enqueue(instr);
                        }
                        else
                            instructionsHeldForNextBatch.Add(instr);
                        break;
                    case InstructionUnit.FpMul:
                        if (fpMulReservationStation.instructions.Count < 3)
                        {
                            fpMulReservationStation.instructions.Enqueue(instr);
                        }
                        else
                            instructionsHeldForNextBatch.Add(instr);
                        break;
                    default:
                        if (memoryUnit.loadBuffers.Count < 5)
                        {
                            memoryUnit.loadBuffers.Enqueue(instr);
                        }
                        else
                            instructionsHeldForNextBatch.Add(instr);
                        break;
                }
            }

            instrQueue.nextBatch = instructionsHeldForNextBatch;
            instrQueue.LoadNextBatch();
        }
    }
}
