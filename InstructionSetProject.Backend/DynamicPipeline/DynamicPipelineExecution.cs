using InstructionSetProject.Backend.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public DynamicPipelineExecution(InstructionList instrList)
        {
            dataStructures = new();
            alu = new(dataStructures);
            instrQueue = new InstructionQueue(instrList, dataStructures.InstructionPointer);
            machineCode = Assembler.Assemble(instrList);
            dataStructures.Memory.AddInstructionCode(machineCode);
            statistics = new();
            dependencyManager = new DependencyManager(dataStructures);
            reorderBuffer = new ReorderBuffer(dataStructures);
            memoryUnit = new MemoryUnit(dataStructures);
            commonDataBus = new();
            fpAdderReservationStation = new();
            fpMulReservationStation = new();
            integerReservationStation = new();
        }

        public void Continue()
        {

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
            statistics.ClockTicks++;
        }

        private void ClockTickCommonDataBus()
        {
            foreach (var instr in commonDataBus)
            {
                dependencyManager.UpdateDependency(instr);
                reorderBuffer.buffers.Enqueue(instr, instr.Index);
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
            integerUnit.result = result;
            var instr = integerUnit;
            integerUnit = null;
            return instr;
        }
    }
}
