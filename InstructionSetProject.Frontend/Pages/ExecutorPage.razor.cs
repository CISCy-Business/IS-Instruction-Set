using System.Collections.Specialized;
using InstructionSetProject.Backend;
using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.StaticFrontend;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Diagram;
using Syncfusion.Blazor.Popups;
using DiagramSegments = Syncfusion.Blazor.Diagram.ConnectorSegmentType;

namespace InstructionSetProject.Frontend.Pages
{
    public partial class ExecutorPage
    {
        ElementReference SyntaxCode;

        private string ExecMachineCode = "";
        private string ExecAssemblyCode = "";
        private string statsString = "";
        private string space = " ";
        private int charCount = 0;
        bool isGranted = true;

        protected override bool ShouldRender()
        {
            return true;
        }

        bool darkModeExecutorPage = FrontendVariables.darkMode;
        public string MemDumpStart { get; set; } = "";

        public string MemDumpContent => SPEx != null
            ? String.Join("",
                SPEx.DataStructures.Memory
                    .GetBytesAtAddress(MemDumpStart != string.Empty ? Convert.ToUInt32(MemDumpStart, 16) : 0)
                    .Select((memByte) => memByte.ToString("X2")))
            : "";

        private StaticPipelineExecution? SPEx;

        public byte[]? MemoryBytes => SPEx != null ? SPEx.DataStructures.Memory.Bytes : null;

        public string r0 => SPEx != null ? SPEx.DataStructures.R0.value.ToString("X4") : "0000";
        public string r1 => SPEx != null ? SPEx.DataStructures.R1.value.ToString("X4") : "0000";
        public string r2 => SPEx != null ? SPEx.DataStructures.R2.value.ToString("X4") : "0000";
        public string r3 => SPEx != null ? SPEx.DataStructures.R3.value.ToString("X4") : "0000";
        public string r4 => SPEx != null ? SPEx.DataStructures.R4.value.ToString("X4") : "0000";
        public string r5 => SPEx != null ? SPEx.DataStructures.R5.value.ToString("X4") : "0000";
        public string r6 => SPEx != null ? SPEx.DataStructures.R6.value.ToString("X4") : "0000";
        public string r7 => SPEx != null ? SPEx.DataStructures.R7.value.ToString("X4") : "0000";
        public string f0 => SPEx != null ? SPEx.DataStructures.F0.value.ToString("X4") : "0000";
        public string f1 => SPEx != null ? SPEx.DataStructures.F1.value.ToString("X4") : "0000";
        public string f2 => SPEx != null ? SPEx.DataStructures.F2.value.ToString("X4") : "0000";
        public string f3 => SPEx != null ? SPEx.DataStructures.F3.value.ToString("X4") : "0000";
        public string f4 => SPEx != null ? SPEx.DataStructures.F4.value.ToString("X4") : "0000";
        public string f5 => SPEx != null ? SPEx.DataStructures.F5.value.ToString("X4") : "0000";
        public string f6 => SPEx != null ? SPEx.DataStructures.F6.value.ToString("X4") : "0000";
        public string f7 => SPEx != null ? SPEx.DataStructures.F7.value.ToString("X4") : "0000";
        public string IP => SPEx != null ? SPEx.DataStructures.InstructionPointer.value.ToString("X4") : "0000";
        public string SP => SPEx != null ? SPEx.DataStructures.StackPointer.value.ToString("X4") : "0000";
        public string FL => SPEx != null ? SPEx.DataStructures.Flags.AsRegisterValue().ToString("X4") : "0000";
        public string PC => SPEx != null ? SPEx.DataStructures.InstructionPointer.value.ToString("X4") : "0000";
        public string MBP => SPEx != null ? SPEx.DataStructures.MemoryBasePointer.value.ToString("X4") : "0000";

        private bool debugRender = false;

        private int connectorCount = 0;
        // Reference to diagram
        SfDiagramComponent diagram;
        // Defines diagram's nodes collection
        public DiagramObjectCollection<Node> NodeCollection { get; set; } = new();
        // Defines diagram's connector collection
        public DiagramObjectCollection<Connector> ConnectorCollection { get; set; } = new();


        private List<byte> machineCode = new();
        private string output { get; set; }
        private string machineCodeString { get; set; }

        private string fileContent = "";

        public bool ShowItem { get; set; } = true;
        private bool Visibility { get; set; } = false;
        private bool errorVis { get; set; } = false;
        private bool ShowButton { get; set; } = false;
        private ResizeDirection[] dialogResizeDirections { get; set; } = new ResizeDirection[] { ResizeDirection.All };

        private async Task LoadFile(InputFileChangeEventArgs e)
        {
            var file = e.File;
            long maxsize = 512000;

            var buffer = new byte[file.Size];
            await file.OpenReadStream(maxsize).ReadAsync(buffer);
            fileContent = System.Text.Encoding.UTF8.GetString(buffer);
            ExecAssemblyCode = fileContent;
        }

        private async Task SaveAssemblyCode()
        {
            byte[] file = System.Text.Encoding.UTF8.GetBytes(ExecAssemblyCode);
            await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", "assemblyCode.txt", "text/plain", file);
        }
        private async Task SaveStats()
        {
            byte[] file = System.Text.Encoding.UTF8.GetBytes(statsString);
            await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", "assemblyStats.txt", "text/plain", file);
        }

        void IncrementCharCount()
        {
            charCount++;
        }

        void ClearCharCount()
        {
            charCount = 0;
        }

        void selectMemDumpText(ushort instrKey)
        {
            debugRender = true;
            MemDumpStart = instrKey.ToString("X4");
            debugRender = true;
        }

        public string Statistics()
        {
            statsString = "";
            if (SPEx == null) return "No Statistics Yet";
            statsString += "Instruction Types\n";
            statsString += "-----------------\n";
            statsString += "R2 Type: " + SPEx.Statistics.R2InstructionCount + "\n";
            statsString += "R3 Type: " + SPEx.Statistics.R3InstructionCount + "\n";
            statsString += "Rm Type: " + SPEx.Statistics.RmInstructionCount + "\n";
            statsString += "Rs Type: " + SPEx.Statistics.RsInstructionCount + "\n";
            statsString += "F2 Type: " + SPEx.Statistics.F2InstructionCount + "\n";
            statsString += "F3 Type: " + SPEx.Statistics.F3InstructionCount + "\n";
            statsString += "Fm Type: " + SPEx.Statistics.FmInstructionCount + "\n\n";

            statsString += "Clock\n";
            statsString += "-----\n";
            statsString += "Total Clock Ticks: " + SPEx.Statistics.ClockTicks + "\n\n";

            statsString += "Flush\n";
            statsString += "-----\n";
            statsString += "Total Flushes: " + SPEx.Statistics.FlushCount;

            return statsString;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (darkModeExecutorPage == true)
            {
                await JSRuntime.InvokeVoidAsync("toggleDarkModeJS", darkModeExecutorPage);
                FrontendVariables.darkModeExecutor = darkModeExecutorPage;
                FrontendVariables.darkModeExecutorChanged = true;
            }
            if (debugRender)
            {
                await JSRuntime.InvokeVoidAsync("selectDebugTab");
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("autoSelectFirstTab");
            }
            debugRender = false;
        }

        async Task ChangedCode()
        {
            await JSRuntime.InvokeVoidAsync("handleKeyPress", SyntaxCode);
        }

        protected override async Task OnInitializedAsync()
        {
            StartupMethod();
            Statistics();
        }

        void StartupMethod()
        {
            ExecMachineCode = FrontendVariables.currentMachineCodeExecutor;
            FrontendVariables.currentMachineCodeExecutor = "";
            ExecAssemblyCode = FrontendVariables.currentAssemblyCodeExecutor;
            FrontendVariables.currentAssemblyCodeExecutor = "";
        }

        void buildCode()
        {
            if (ExecAssemblyCode.Length != 0)
            {
                try
                {
                    machineCode = Assembler.Assemble(ExecAssemblyCode);
                    string hexCode = BitConverter.ToString(machineCode.ToArray());
                    ExecMachineCode = hexCode.Replace("-", " ");
                    output = "";
                }
                catch (Exception ex)
                {
                    output = "ERROR: " + ex.Message + "\n";
                }

            }
            else
            {
                errorVis = true;
            }
        }

        void runCode()
        {
            try
            {
                SPEx = (StaticPipelineExecution)StaticPipelineExecutor.Execute(ExecAssemblyCode);
                output = "";
            }
            catch (Exception ex)
            {
                output = "ERROR: " + ex.Message + "\n";
            }

            debugRender = true;
            try
            {
                SPEx.Continue();
                output = "";
            }
            catch (Exception ex)
            {
                output = "ERROR: " + ex.Message + "\n";
            }
            debugRender = true;
            Statistics();
        }

        void Debug()
        {
            try
            {
                SPEx = (StaticPipelineExecution)StaticPipelineExecutor.Execute(ExecAssemblyCode);
                output = "";
            }
            catch (Exception ex)
            {
                output = "ERROR: " + ex.Message + "\n";
            }
            OnItemClick();
            debugRender = true;
            JSRuntime.InvokeVoidAsync("debugScrollToTop");
            UpdateDiagram();
        }

        bool IsSelectedFetch(IInstruction instr) => instr == SPEx.fetchingInstruction;
        bool IsSelectedDecode(IInstruction instr) => instr == SPEx.decodingInstruction;
        bool IsSelectedExecute(IInstruction instr) => instr == SPEx.executingInstruction;
        bool IsSelectedMemory(IInstruction instr) => instr == SPEx.memoryInstruction;
        bool IsSelectedWrite(IInstruction instr) => instr == SPEx.writingBackInstruction;

        string DivCSS(IInstruction instr) => IsSelectedFetch(instr) ? "bg-fetch text-white" : (IsSelectedDecode(instr) ? "bg-decode text-white" : (IsSelectedExecute(instr) ? "bg-execute text-white" : (IsSelectedMemory(instr) ? "bg-memory text-white" : (IsSelectedWrite(instr) ? "bg-write text-white" : (FrontendVariables.darkMode ? "bg-dark-mode" : "bg-white")))));

        void ClockTick()
        {
            debugRender = true;
            try
            {
                SPEx.ClockTick();
                output = "";
            }
            catch (Exception ex)
            {
                output = "ERROR: " + ex.Message + "\n";
            }
            //JSRuntime.InvokeVoidAsync("stepScroll");
            Statistics();
            debugRender = true;
            UpdateDiagram();
        }

        void step()
        {
            debugRender = true;
            try
            {
                SPEx.Step();
                output = "";
            }
            catch (Exception ex)
            {
                output = "ERROR: " + ex.Message + "\n";
            }
            //JSRuntime.InvokeVoidAsync("stepScroll");
            Statistics();
            debugRender = true;
            UpdateDiagram();
        }

        void Continue()
        {
            debugRender = true;
            try
            {
                SPEx.Continue();
                output = "";
            }
            catch (Exception ex)
            {
                output = "ERROR: " + ex.Message + "\n";
            }
            Statistics();
            debugRender = true;
        }

        void Stop()
        {
            SPEx = null;
            InitDiagramModel();
        }

        #region ColorCodes

        private const string Purple = "#7300ff";
        private const string Blue = "#0004FF";
        private const string Black = "black";
        private const string LightBlue = "#0099FF";
        private const string Green = "#00CC44";
        private const string Yellow = "#B6BF02";
        private const string ControlDash = "4,2";
        #endregion

        void UpdateDiagram()
        {
            if (SPEx != null && ConnectorCollection.Count != 0)
            {
                UpdateFetchStage();
                UpdateDecodeStage();
                UpdateExecuteStage();
                UpdateMemoryStage();
                UpdateWritebackStage();
                UpdateLabels();
            }
        }

        public string fetchMnemonic => (SPEx != null && SPEx.fetchingInstruction != null) ? SPEx.fetchingInstruction.GetMnemonic() : "";
        public string decodeMnemonic => (SPEx != null && SPEx.decodingInstruction != null) ? SPEx.decodingInstruction.GetMnemonic() : "";
        public string executeMnemonic => (SPEx != null && SPEx.executingInstruction != null) ? SPEx.executingInstruction.GetMnemonic() : "";
        public string memoryMnemonic => (SPEx != null && SPEx.memoryInstruction != null) ? SPEx.memoryInstruction.GetMnemonic() : "";
        public string writeMnemonic => (SPEx != null && SPEx.writingBackInstruction != null) ? SPEx.writingBackInstruction.GetMnemonic() : "";

        void UpdateLabels()
        {
            idExRsD1.Content = SPEx != null && SPEx.DecodeExecute.ReadData1 != null
                ? SPEx.DecodeExecute.ReadData1?.ToString("X4")
                : "null";

            idExRsD2.Content = SPEx != null && SPEx.DecodeExecute.ReadData2 != null
                ? SPEx.DecodeExecute.ReadData2?.ToString("X4")
                : "null";

            idExImmediate.Content = SPEx != null && SPEx.DecodeExecute.Immediate != null
                ? SPEx.DecodeExecute.Immediate?.ToString("X4")
                : "null";

            idExRd.Content = SPEx != null && SPEx.DecodeExecute.WriteRegister != null
                ? SPEx.DecodeExecute.WriteRegister.Label
                : "null";

            exMemAluResult.Content = SPEx != null && SPEx.ExecuteMemory.AluResult != null
                ? SPEx.ExecuteMemory.AluResult?.ToString("X4")
                : "null";

            exMemRsD2.Content = SPEx != null && SPEx.ExecuteMemory.ReadData2 != null
                ? SPEx.ExecuteMemory.ReadData2?.ToString("X4")
                : "null";

            exMemRd.Content = SPEx != null && SPEx.ExecuteMemory.WriteRegister != null
                ? SPEx.ExecuteMemory.WriteRegister.Label
                : "null";

            memWbMemData.Content = SPEx != null && SPEx.MemoryWriteBack.ReadData != null
                ? SPEx.MemoryWriteBack.ReadData?.ToString("X4")
                : "null";

            memWbAluResult.Content = SPEx != null && SPEx.MemoryWriteBack.AluResult != null
                ? SPEx.MemoryWriteBack.AluResult?.ToString("X4")
                : "null";

            memWbRd.Content = SPEx != null && SPEx.MemoryWriteBack.WriteRegister != null
                ? SPEx.MemoryWriteBack.WriteRegister.Label
                : "null";

            rdReg.Content = SPEx != null && SPEx.MemoryWriteBack.WriteRegister != null
                ? SPEx.MemoryWriteBack.WriteRegister.Label
                : "null";
        }

        void UpdateFetchStage()
        {
            var addToMux = GetConnectorByID(AddPCToFetchMux);
            var instrSize = GetConnectorByID(IntrMemToAddPC);
            var instrMemToReg = GetConnectorByID(IntrMemToIFID);
            var pcToAdd = GetConnectorByID(PCToAddPC);
            var instrMemToAdd = GetConnectorByID(PCToInstrMemIn);
            var muxToPc = GetConnectorByID(FetchMuxToPCIn);

            if (SPEx?.fetchingInstruction == null)
            {
                DisableLines(new List<Connector> { addToMux, instrSize, instrMemToReg, pcToAdd, instrMemToAdd, muxToPc });
                return;
            }

            var linesToEnable = new List<Connector>();
            var linesToDisable = new List<Connector>();

            linesToEnable.Add(addToMux);
            linesToEnable.Add(instrSize);
            linesToEnable.Add(instrMemToReg);
            linesToEnable.Add(pcToAdd);
            linesToEnable.Add(instrMemToAdd);
            linesToEnable.Add(addToMux);
            linesToEnable.Add(muxToPc);

            EnableLines(linesToEnable, Purple);
            DisableLines(linesToDisable);
        }

        void UpdateDecodeStage()
        {
            var regToControlBits = GetConnectorByID(IFIDToControl);
            var regToRs1 = GetConnectorByID(IFIDToRegIn0);
            var regToRs2 = GetConnectorByID(IFIDToRegIn1);
            var regToImm = GetConnectorByID(IFIDToImmGen);
            var regToRd = GetConnectorByID(IFIDToIDEX);
            var rd1 = GetConnectorByID(RegToIDEXIn1);
            var rd2 = GetConnectorByID(RegToIDEXIn2);
            var immResult = GetConnectorByID(ImmGenToIDEX);

            var instr = SPEx?.decodingInstruction;
            if (instr == null)
            {
                DisableLines(new List<Connector> { regToControlBits, regToRs1, regToRs2, regToImm, regToRd, rd1, rd2, immResult });
                return;
            }

            var linesToEnable = new List<Connector>();
            var linesToDisable = new List<Connector>();

            linesToEnable.Add(regToControlBits);

            if (instr.firstRegisterType == RegisterType.Read || (instr.firstRegisterType == RegisterType.Write &&
                                                                 instr.secondRegisterType == RegisterType.Read))
            {
                linesToEnable.Add(regToRs1);
                linesToEnable.Add(rd1);
            }
            else
            {
                linesToDisable.Add(regToRs1);
                linesToDisable.Add(rd1);
            }

            if (instr.thirdRegisterType == RegisterType.Read || (instr.firstRegisterType == RegisterType.Read &&
                                                                 instr.secondRegisterType == RegisterType.Read))
            {
                linesToEnable.Add(regToRs2);
                linesToEnable.Add(rd2);
            }
            else
            {
                linesToDisable.Add(regToRs2);
                linesToDisable.Add(rd2);
            }

            if (instr.immediate != null)
            {
                linesToEnable.Add(regToImm);
                linesToEnable.Add(immResult);
            }
            else
            {
                linesToDisable.Add(regToImm);
                linesToDisable.Add(immResult);
            }

            if (instr.firstRegisterType == RegisterType.Write)
                linesToEnable.Add(regToRd);
            else
                linesToDisable.Add(regToRd);

            EnableLines(linesToEnable, Blue);
            DisableLines(linesToDisable);

        }

        void UpdateExecuteStage()
        {
            var aluSrc = GetConnectorByID(ALUSToExecuteMux);
            var rsd1ToAlu = GetConnectorByID(IDEXToALU);
            var rsd2ToMux = GetConnectorByID(IDEXToExecuteMux1);
            var immToMux = GetConnectorByID(IDEXToExecuteMux0);
            var muxToAlu = GetConnectorByID(ExecuteMuxToALU);
            var rsd2ToReg = GetConnectorByID(IDEXToEXMEM2);
            var rd = GetConnectorByID(IDEXToEXMEM3);
            var tmpFlags = GetConnectorByID(ALUToEXMEM0);
            var aluResult = GetConnectorByID(ALUToEXMEM1);

            var instr = SPEx?.executingInstruction;
            if (instr == null)
            {
                DisableLines(new List<Connector> { aluSrc, rsd1ToAlu, rsd2ToMux, immToMux, muxToAlu, rsd2ToReg, rd, tmpFlags, aluResult });
                return;
            }

            var linesToEnable = new List<Connector>();
            var linesToDisable = new List<Connector>();

            if (instr.controlBits.ALUSrc)
                linesToEnable.Add(aluSrc);
            else
                linesToDisable.Add(aluSrc);

            if (SPEx?.DecodeExecute.ReadData1 != null)
                linesToEnable.Add(rsd1ToAlu);
            else
                linesToDisable.Add(rsd1ToAlu);

            if (!instr.controlBits.ALUSrc && SPEx?.DecodeExecute.ReadData2 != null)
            {
                linesToEnable.Add(rsd2ToMux);
                linesToDisable.Add(rsd2ToReg);
            }
            else if (SPEx?.DecodeExecute.ReadData2 != null)
            {
                linesToEnable.Add(rsd2ToReg);
                linesToDisable.Add(rsd2ToMux);
            }
            else
            {
                linesToDisable.Add(rsd2ToMux);
                linesToDisable.Add(rsd2ToReg);
            }

            if (instr.controlBits.ALUSrc)
                linesToEnable.Add(immToMux);
            else
                linesToDisable.Add(immToMux);

            linesToEnable.Add(muxToAlu);

            if (instr.firstRegisterType == RegisterType.Write)
                linesToEnable.Add(rd);
            else
                linesToDisable.Add(rd);

            if (instr.aluOperation != AluOperation.NoOperation)
            {
                linesToEnable.Add(tmpFlags);
                linesToEnable.Add(aluResult);
            }
            else
            {
                linesToDisable.Add(tmpFlags);
                linesToDisable.Add(aluResult);
            }

            EnableLines(linesToEnable, LightBlue);
            DisableLines(linesToDisable);

        }

        void UpdateMemoryStage()
        {
            var pcSrc = GetConnectorByID(PCS1ToCheckFlags);
            var memWrite = GetConnectorByID(MW1ToDataMem);
            var memRead = GetConnectorByID(MR1ToDataMem);
            var flags = GetConnectorByID(EXMEMToCheckFlags);
            var addr = GetConnectorByID(EXMEMToDataMem0);
            var rsd2ToMem = GetConnectorByID(EXMEMToDataMem1);
            var aluResult = GetConnectorByID(EXMEMToMEMWB1);
            var rd = GetConnectorByID(EXMEMToMEMWB2);
            var checkFlagsToPCMux = GetConnectorByID(CheckFlagsToFlgRet);
            var checkFlagsToPCMux2 = GetConnectorByID(FlgRetToFetchMux);
            var immToPC = GetConnectorByID(IDEXToFetchMux);
            var dataOutput = GetConnectorByID(DataMemToMEMWB);

            var instr = SPEx?.memoryInstruction;
            if (instr == null)
            {
                DisableLines(new List<Connector> { pcSrc, memWrite, memRead, flags, addr, rsd2ToMem, aluResult, rd, checkFlagsToPCMux, checkFlagsToPCMux2, immToPC, dataOutput });
                return;
            }

            var linesToEnable = new List<Connector>();
            var linesToDisable = new List<Connector>();

            if (instr.controlBits.PCSrc)
                linesToEnable.Add(pcSrc);
            else
                linesToDisable.Add(pcSrc);

            if (instr.controlBits.MemWrite)
            {
                linesToEnable.Add(memWrite);
                linesToEnable.Add(rsd2ToMem);
            }
            else
            {
                linesToDisable.Add(memWrite);
                linesToDisable.Add(rsd2ToMem);
            }

            if (instr.controlBits.MemRead)
            {
                linesToEnable.Add(memRead);
                linesToEnable.Add(dataOutput);
            }
            else
            {
                linesToDisable.Add(memRead);
                linesToDisable.Add(dataOutput);
            }

            if (SPEx?.ExecuteMemory.FlagsRegister != null && instr.controlBits.PCSrc)
            {
                linesToEnable.Add(flags);
                linesToEnable.Add(checkFlagsToPCMux);
                linesToEnable.Add(checkFlagsToPCMux2);
                linesToEnable.Add(immToPC);
            }
            else
            {
                linesToDisable.Add(flags);
                linesToDisable.Add(checkFlagsToPCMux);
                linesToDisable.Add(checkFlagsToPCMux2);
                linesToDisable.Add(immToPC);
            }

            if (instr.controlBits.MemRead || instr.controlBits.MemWrite)
                linesToEnable.Add(addr);
            else
                linesToDisable.Add(addr);

            if (SPEx?.ExecuteMemory.AluResult != null && !instr.controlBits.MemRead && !instr.controlBits.MemWrite)
                linesToEnable.Add(aluResult);
            else
                linesToDisable.Add(aluResult);

            if (SPEx?.ExecuteMemory.WriteRegister != null)
                linesToEnable.Add(rd);
            else
                linesToDisable.Add(rd);

            EnableLines(linesToEnable, Green);
            DisableLines(linesToDisable);

        }

        void UpdateWritebackStage()
        {
            var regWrite = GetConnectorByID(RW2ToRWRet);
            var regWrite2 = GetConnectorByID(RWRetToReg);
            var memToReg = GetConnectorByID(MTR2ToWriteMux);
            var memResult = GetConnectorByID(MEMWBToWriteMux1);
            var aluResult = GetConnectorByID(MEMWBToWriteMux2);
            var rd = GetConnectorByID(MEMWBToRdRet);
            var rd2 = GetConnectorByID(RdRetToReg);
            var muxToReg = GetConnectorByID(WriteMuxToReg);

            var instr = SPEx?.writingBackInstruction;
            if (instr == null)
            {
                DisableLines(new List<Connector> { regWrite, regWrite2, memToReg, memResult, aluResult, rd, rd2, muxToReg });
                return;
            }

            var linesToEnable = new List<Connector>();
            var linesToDisable = new List<Connector>();

            if (instr.controlBits.RegWrite)
            {
                linesToEnable.Add(regWrite);
                linesToEnable.Add(regWrite2);
                linesToEnable.Add(rd);
                linesToEnable.Add(rd2);
                linesToEnable.Add(muxToReg);
            }
            else
            {
                linesToDisable.Add(regWrite);
                linesToDisable.Add(regWrite2);
                linesToDisable.Add(rd);
                linesToDisable.Add(rd2);
                linesToDisable.Add(muxToReg);
            }

            if (instr.controlBits.MemToReg)
            {
                linesToEnable.Add(memToReg);
                linesToEnable.Add(memResult);
                linesToDisable.Add(aluResult);
            }
            else
            {
                linesToDisable.Add(memToReg);
                linesToDisable.Add(memResult);
                linesToEnable.Add(aluResult);
            }

            EnableLines(linesToEnable, Yellow);
            DisableLines(linesToDisable);

        }

        Connector GetConnectorByID(string id)
        {
            var connector = ConnectorCollection.First(connector => connector.ID == id);
            if (connector == null)
                throw new Exception($"Connector not found with ID: {id}");
            return connector;
        }

        void EnableLines(List<Connector> lines, string color)
        {
            foreach (var line in lines)
            {
                line.Style.StrokeColor = color;
                line.Style.StrokeWidth = 3;
            }
        }

        void DisableLines(List<Connector> lines)
        {
            foreach (var line in lines)
            {
                line.Style.StrokeColor = Black;
                line.Style.StrokeWidth = 1;
            }
        }

        #region DataLabels
        private PathAnnotation idExRsD1 = new();
        private PathAnnotation idExRsD2 = new();
        private PathAnnotation idExImmediate = new();
        private PathAnnotation idExRd = new();
        private PathAnnotation exMemAluResult = new();
        private PathAnnotation exMemRsD2 = new();
        private PathAnnotation exMemRd = new();
        private PathAnnotation memWbMemData = new();
        private PathAnnotation memWbAluResult = new();
        private PathAnnotation memWbRd = new();
        private ShapeAnnotation rdReg = new();
        #endregion

        private void InitDiagramModel()
        {
            NodeCollection = new DiagramObjectCollection<Node>();
            ConnectorCollection = new DiagramObjectCollection<Connector>();

            #region Ports
            // Fetch Ports
            List<PointPort> FetchMuxPorts = new List<PointPort>();
            FetchMuxPorts.Add(AddPort("portFetchMuxIn0", 0.15, 0.01));
            FetchMuxPorts.Add(AddPort("portFetchMuxIn1", 0.85, 0.01));
            FetchMuxPorts.Add(AddPort("portFetchMuxIn2", 1, 0.5));
            FetchMuxPorts.Add(AddPort("portFetchMuxOut0", 0.5, 1));
            List<PointPort> PCPorts = new List<PointPort>();
            PCPorts.Add(AddPort("portPCIn", 0.01, 0.5));
            PCPorts.Add(AddPort("portPCOut", 1, 0.5));
            List<PointPort> InstrMemPorts = new List<PointPort>();
            InstrMemPorts.Add(AddPort("portInstrMemIn", 0.01, 0.1));
            InstrMemPorts.Add(AddPort("portInstrMemOut", 1, 0.5));
            InstrMemPorts.Add(AddPort("portInstrMemOut1", 0.7, 0.01));
            List<PointPort> AddPCPorts = new List<PointPort>();
            AddPCPorts.Add(AddPort("portAddPCIn0", 0.15, 0.01));
            AddPCPorts.Add(AddPort("portAddPCIn1", 0.85, 0.01));
            AddPCPorts.Add(AddPort("portAddPCOut0", 0.5, 1));

            List<PointPort> ifidPorts = new List<PointPort>();
            //ifidPorts.Add(AddPort("portIfidIn0", 0.01, 0.15));
            ifidPorts.Add(AddPort("portIfidIn1", 0.01, 0.5));
            //ifidPorts.Add(AddPort("portIfidOut0", 1, 0.15));
            ifidPorts.Add(AddPort("portIfidOut1", 1, 0.5));

            // Decode Ports
            List<PointPort> regPorts = new List<PointPort>();
            regPorts.Add(AddPort("portRegIn0", 0.01, 0.1));
            regPorts.Add(AddPort("portRegIn1", 0.01, 0.35));
            regPorts.Add(AddPort("portRegIn2", 0.01, 0.7));
            regPorts.Add(AddPort("portRegIn3", 0.1, 1));
            regPorts.Add(AddPort("portRegIn4", 0.4, 0.01));
            regPorts.Add(AddPort("portRegOut0", 1, 0.15));
            regPorts.Add(AddPort("portRegOut1", 1, 0.5));
            List<PointPort> ImmGenPorts = new List<PointPort>();
            ImmGenPorts.Add(AddPort("portImmGenIn", 0.01, 0.5));
            ImmGenPorts.Add(AddPort("portImmGenOut", 1, 0.5));
            List<PointPort> ControlPorts = new List<PointPort>();
            ControlPorts.Add(AddPort("portControlIn", 0.01, 0.5));
            ControlPorts.Add(AddPort("portControlOut1", 1, 0.5));

            List<PointPort> idexPorts = new List<PointPort>();
            //idexPorts.Add(AddPort("portIdexIn0", 0.01, 0.15));
            idexPorts.Add(AddPort("portIdexIn1", 0.01, 0.438));
            idexPorts.Add(AddPort("portIdexIn2", 0.01, 0.515));
            idexPorts.Add(AddPort("portIdexIn3", 0.01, 0.738));
            idexPorts.Add(AddPort("portIdexIn4", 0.01, 0.87));
            //idexPorts.Add(AddPort("portIdexOut0", 1, 0.15));
            idexPorts.Add(AddPort("portIdexOut1", 1, 0.438));
            idexPorts.Add(AddPort("portIdexOut2", 1, 0.515));
            idexPorts.Add(AddPort("portIdexOut3", 1, 0.738));
            idexPorts.Add(AddPort("portIdexOut4", 1, 0.87));
            List<PointPort> RWPorts = new List<PointPort>();
            RWPorts.Add(AddPort("portRWIn", 0.01, 0.5));
            RWPorts.Add(AddPort("portRWOut", 1, 0.5));
            List<PointPort> MTRPorts = new List<PointPort>();
            MTRPorts.Add(AddPort("portMTRIn", 0.01, 0.5));
            MTRPorts.Add(AddPort("portMTROut", 1, 0.5));
            List<PointPort> MRPorts = new List<PointPort>();
            MRPorts.Add(AddPort("portMRIn", 0.01, 0.5));
            MRPorts.Add(AddPort("portMROut", 1, 0.5));
            List<PointPort> MWPorts = new List<PointPort>();
            MWPorts.Add(AddPort("portMWIn", 0.01, 0.5));
            MWPorts.Add(AddPort("portMWOut", 1, 0.5));
            List<PointPort> PCSPorts = new List<PointPort>();
            PCSPorts.Add(AddPort("portPCSIn", 0.01, 0.5));
            PCSPorts.Add(AddPort("portPCSOut", 1, 0.5));
            List<PointPort> ALUSPorts = new List<PointPort>();
            ALUSPorts.Add(AddPort("portALUSIn", 0.01, 0.5));
            ALUSPorts.Add(AddPort("portALUSOut", 1, 0.5));

            // Execute Ports
            List<PointPort> ExecuteMuxPorts = new List<PointPort>();
            ExecuteMuxPorts.Add(AddPort("portExecuteMuxIn0", 0, 0.5));
            ExecuteMuxPorts.Add(AddPort("portExecuteMuxIn1", 0.85, 0.01));
            ExecuteMuxPorts.Add(AddPort("portExecuteMuxIn2", 1, 0.5));
            ExecuteMuxPorts.Add(AddPort("portExecuteMuxOut0", 0.5, 1));
            List<PointPort> AddSumPorts = new List<PointPort>();
            AddSumPorts.Add(AddPort("portAddSumIn0", 0.15, 0.01));
            AddSumPorts.Add(AddPort("portAddSumIn1", 0.85, 0.01));
            AddSumPorts.Add(AddPort("portAddSumOut", 0.5, 1));
            List<PointPort> ALUPorts = new List<PointPort>();
            ALUPorts.Add(AddPort("portALUIn0", 0.14, 0.01));
            ALUPorts.Add(AddPort("portALUIn1", 0.86, 0.01));
            ALUPorts.Add(AddPort("portALUOut0", 0.33, 1));
            ALUPorts.Add(AddPort("portALUOut1", 0.65, 1));

            List<PointPort> exmemPorts = new List<PointPort>();
            exmemPorts.Add(AddPort("portExmemIn0", 0.01, 0.16));
            exmemPorts.Add(AddPort("portExmemIn1", 0.01, 0.39));
            exmemPorts.Add(AddPort("portExmemIn2", 0.01, 0.738));
            exmemPorts.Add(AddPort("portExmemIn3", 0.01, 0.87));
            exmemPorts.Add(AddPort("portExmemOut0", 1, 0.16));
            exmemPorts.Add(AddPort("portExmemOut1", 1, 0.39));
            exmemPorts.Add(AddPort("portExmemOut2", 1, 0.738));
            exmemPorts.Add(AddPort("portExmemOut3", 1, 0.87));
            List<PointPort> RW1Ports = new List<PointPort>();
            RW1Ports.Add(AddPort("portRW1In", 0.01, 0.5));
            RW1Ports.Add(AddPort("portRW1Out", 1, 0.5));
            List<PointPort> MTR1Ports = new List<PointPort>();
            MTR1Ports.Add(AddPort("portMTR1In", 0.01, 0.5));
            MTR1Ports.Add(AddPort("portMTR1Out", 1, 0.5));
            List<PointPort> MR1Ports = new List<PointPort>();
            MR1Ports.Add(AddPort("portMR1In", 0.01, 0.5));
            MR1Ports.Add(AddPort("portMR1Out", 1, 0.5));
            List<PointPort> MW1Ports = new List<PointPort>();
            MW1Ports.Add(AddPort("portMW1In", 0.01, 0.5));
            MW1Ports.Add(AddPort("portMW1Out", 1, 0.5));
            List<PointPort> PCS1Ports = new List<PointPort>();
            PCS1Ports.Add(AddPort("portPCS1In", 0.01, 0.5));
            PCS1Ports.Add(AddPort("portPCS1Out", 1, 0.5));

            // Memory Ports
            List<PointPort> dataMemPorts = new List<PointPort>();
            dataMemPorts.Add(AddPort("portDataMemIn0", 0.01, 0.25));
            dataMemPorts.Add(AddPort("portDataMemIn1", 0.01, 0.75));
            dataMemPorts.Add(AddPort("portDataMemIn2", 0.25, 0.01));
            dataMemPorts.Add(AddPort("portDataMemIn3", 0.75, 0.01));
            dataMemPorts.Add(AddPort("portDataMemOut", 1, 0.25));
            List<PointPort> ChkFlgPorts = new List<PointPort>();
            ChkFlgPorts.Add(AddPort("portChkFlgIn0", 0.15, 0.01));
            ChkFlgPorts.Add(AddPort("portChkFlgIn1", 0.85, 0.01));
            ChkFlgPorts.Add(AddPort("portChkFlgOut0", 0.5, 1));

            List<PointPort> memwbPorts = new List<PointPort>();
            memwbPorts.Add(AddPort("portMemwbIn0", 0.01, 0.39));
            memwbPorts.Add(AddPort("portMemwbIn1", 0.01, 0.80));
            memwbPorts.Add(AddPort("portMemwbIn2", 0.01, 0.87));
            memwbPorts.Add(AddPort("portMemwbOut0", 1, 0.39));
            memwbPorts.Add(AddPort("portMemwbOut1", 1, 0.80));
            memwbPorts.Add(AddPort("portMemwbOut2", 1, 0.87));
            List<PointPort> RW2Ports = new List<PointPort>();
            RW2Ports.Add(AddPort("portRW2In", 0.01, 0.5));
            RW2Ports.Add(AddPort("portRW2Out", 1, 0.5));
            List<PointPort> MTR2Ports = new List<PointPort>();
            MTR2Ports.Add(AddPort("portMTR2In", 0.01, 0.5));
            MTR2Ports.Add(AddPort("portMTR2Out", 1, 0.5));

            List<PointPort> FLRetPorts = new List<PointPort>();
            FLRetPorts.Add(AddPort("portFLRetIn", 1, 0.5));
            FLRetPorts.Add(AddPort("portFLRetOut", 0.01, 0.5));

            // Write Ports
            List<PointPort> WriteMuxPorts = new List<PointPort>();
            WriteMuxPorts.Add(AddPort("portWriteMuxIn0", 0.15, 0.01));
            WriteMuxPorts.Add(AddPort("portWriteMuxIn1", 0.85, 0.01));
            WriteMuxPorts.Add(AddPort("portWriteMuxIn2", 1, 0.5));
            WriteMuxPorts.Add(AddPort("portWriteMuxOut0", 0.5, 1));

            List<PointPort> RdRegReturnPorts = new List<PointPort>();
            RdRegReturnPorts.Add(AddPort("portRdRetIn", 1, 0.5));
            RdRegReturnPorts.Add(AddPort("portRdRetOut", 0.01, 0.5));

            List<PointPort> RWRetPorts = new List<PointPort>();
            RWRetPorts.Add(AddPort("portRWRetIn", 1, 0.5));
            RWRetPorts.Add(AddPort("portRWRetOut", 0.01, 0.5));

            // Window Sizing Ports
            List<PointPort> WinSizePorts = new List<PointPort>();
            #endregion

            string blueColor = "#0875F5";

            #region Nodes
            // Fetch Nodes
            CreateNode("FetchMux", 60, 293, 60, 27, -90, 90, FetchMuxPorts, FlowShapeType.Terminator, "Mux", "white", "8,0", "black");
            CreateNode("PC", 110, 293, 25, 55, 0, 0, PCPorts, FlowShapeType.Process, "PC", "white", "8,0", "black");
            CreateNode("InstrMem", 200, 370, 100, 100, 0, 0, InstrMemPorts, FlowShapeType.Process, "Instruction Memory", "white", "8,0", "black");
            CreateNode("AddPC", 205, 250, 75, 70, -90, 90, AddPCPorts, BasicShapeType.Trapezoid, "Add", "white", "8,0", "black");

            CreateNode("IFID", 300, 343, 30, 450, 0, -90, ifidPorts, FlowShapeType.Process, "IF/ID", "white", "8,0", "black", default, new DiagramPoint(.5, .1));

            // Decode Nodes
            rdReg = new ShapeAnnotation()
            {
                Content = "null",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Offset = new DiagramPoint(0.1, .71),
                Style = new TextStyle() {Color = "red", FontSize = 9.0 }
            };
            CreateNode("Registers", 450, 350, 100, 100, 0, 0, regPorts, FlowShapeType.Process, "Registers", "white", "8,0", "black", default, null, rdReg);
            CreateNode("ImmGen", 470, 450, 40, 75, 0, 0, ImmGenPorts, BasicShapeType.Ellipse, "Imm Gen", "white", "8,0", "black");
            CreateNode("Control", 480, 70, 45, 100, 0, 0, ControlPorts, BasicShapeType.Ellipse, "Control", "white", ControlDash, "black");

            CreateNode("IDEX", 560, 343, 30, 450, 0, -90, idexPorts, FlowShapeType.Process, "ID/EX", "white", "8,0", "black", default, new DiagramPoint(.5, .1));
            CreateNode("RW", 560, 33, 35, 15, 0, 0, RWPorts, FlowShapeType.Process, "RW", "white", ControlDash, "black");
            CreateNode("MTR", 560, 48, 35, 15, 0, 0, MTRPorts, FlowShapeType.Process, "MTR", "white", ControlDash, "black");
            CreateNode("MR", 560, 63, 35, 15, 0, 0, MRPorts, FlowShapeType.Process, "MR", "white", ControlDash, "black");
            CreateNode("MW", 560, 78, 35, 15, 0, 0, MWPorts, FlowShapeType.Process, "MW", "white", ControlDash, "black");
            CreateNode("PCS", 560, 93, 35, 15, 0, 0, PCSPorts, FlowShapeType.Process, "PCS", "white", ControlDash, "black");
            CreateNode("ALUS", 560, 108, 35, 15, 0, 0, ALUSPorts, FlowShapeType.Process, "ALUS", "white", ControlDash, "black");

            // Execute Nodes
            CreateNode("ExecuteMux", 632, 378, 60, 27, -90, 90, ExecuteMuxPorts, FlowShapeType.Terminator, "Mux", "white", "8,0", "black");
            CreateNode("ALU", 710, 281, 75, 70, -90, 90, ALUPorts, BasicShapeType.Trapezoid, "ALU", "white", "8,0", "black");
            //CreateNode("ALUControl", 640, 490, 45, 75, 0, 0, ImmGenPorts, BasicShapeType.Ellipse, "ALU Control", "white", ControlOpacity, "black");

            CreateNode("EXMEM", 800, 343, 30, 450, 0, -90, exmemPorts, FlowShapeType.Process, "EX/MEM", "white", "8,0", "black", default, new DiagramPoint(.5, .1));
            CreateNode("RW1", 800, 48, 35, 15, 0, 0, RW1Ports, FlowShapeType.Process, "RW", "white", ControlDash, "black");
            CreateNode("MTR1", 800, 63, 35, 15, 0, 0, MTR1Ports, FlowShapeType.Process, "MTR", "white", ControlDash, "black");
            CreateNode("MR1", 800, 78, 35, 15, 0, 0, MR1Ports, FlowShapeType.Process, "MR", "white", ControlDash, "black");
            CreateNode("MW1", 800, 93, 35, 15, 0, 0, MW1Ports, FlowShapeType.Process, "MW", "white", ControlDash, "black");
            CreateNode("PCS1", 800, 108, 35, 15, 0, 0, PCS1Ports, FlowShapeType.Process, "PCS", "white", ControlDash, "black");

            // Memory Nodes
            CreateNode("DataMem", 950, 319, 100, 100, 0, 0, dataMemPorts, FlowShapeType.Process, "Data Memory", "white", "8,0", "black");
            CreateNode("CheckFlags", 880, 164, 75, 65, -90, 90, ChkFlgPorts, BasicShapeType.Trapezoid, "Check Flgs", "white", ControlDash, "black");

            CreateNode("MEMWB", 1055, 343, 30, 450, 0, -90, memwbPorts, FlowShapeType.Process, "MEM/WB", "white", "8,0", "black", default, new DiagramPoint(.5, .1));
            CreateNode("RW2", 1055, 93, 35, 15, 0, 0, RW2Ports, FlowShapeType.Process, "RW", "white", ControlDash, "black");
            CreateNode("MTR2", 1055, 108, 35, 15, 0, 0, MTR2Ports, FlowShapeType.Process, "MTR", "white", ControlDash, "black");

            CreateNode("FlgReturn", 750, 10, 1, 1, 0, 0, FLRetPorts, FlowShapeType.Process, "", "white", "8,0", "black");

            // Write Nodes
            CreateNode("WriteMux", 1125, 315, 60, 27, -90, 90, WriteMuxPorts, FlowShapeType.Terminator, "Mux", "white", "8,0", "black");
            CreateNode("RdRegReturn", 750, 635, 1, 1, 0, 0, RdRegReturnPorts, FlowShapeType.Process, "", "white", "8,0", "black");
            CreateNode("RWReturn", 750, 15, 1, 1, 0, 0, RWRetPorts, FlowShapeType.Process, "", "white", "8,0", "black");

            // Window Sizing Node
            CreateNode("sizeNodeYX", 1170, 650, 1, 1, 0, 0, WinSizePorts, FlowShapeType.Process, "", "white", "8,0", "white");
            #endregion

            #region Segments
            OrthogonalSegment segment1 = new OrthogonalSegment()
            {
                Type = DiagramSegments.Orthogonal,
                Length = 30,
                Direction = Direction.Right
            };
            OrthogonalSegment segment2 = new OrthogonalSegment()
            {
                Type = DiagramSegments.Orthogonal,
                Length = 300,
                Direction = Direction.Bottom
            };
            OrthogonalSegment segment3 = new OrthogonalSegment()
            {
                Type = DiagramSegments.Orthogonal,
                Length = 30,
                Direction = Direction.Left
            };
            OrthogonalSegment segment4 = new OrthogonalSegment()
            {
                Type = DiagramSegments.Orthogonal,
                Length = 200,
                Direction = Direction.Top
            };
            #endregion

            #region Connectors
            // Fetch Connectors
            CreateConnector(FetchMuxToPCIn, "FetchMux", "portFetchMuxOut0", "PC", "portPCIn", "8,0", "black");
            CreateConnector(PCToInstrMemIn, "PC", "portPCOut", "InstrMem", "portInstrMemIn", "8,0", "black");
            CreateConnector(PCToAddPC, "PC", "portPCOut", "AddPC", "portAddPCIn1", "8,0", "black");
            CreateConnector(AddPCToFetchMux, "AddPC", "portAddPCOut0", "FetchMux", "portFetchMuxIn2", "8,0", "black", "0", AnnotationAlignment.Center, .78);
            CreateConnector(IntrMemToIFID, "InstrMem", "portInstrMemOut", "IFID", "portIfidIn1", "8,0", "black");
            CreateConnector(IntrMemToAddPC, "InstrMem", "portInstrMemOut1", "AddPC", "portAddPCIn0", "8,0", "black", "Instr Size", AnnotationAlignment.Center, 0.15);

            // Decode Connectors
            CreateConnector(IFIDToRegIn0, "IFID", "portIfidOut1", "Registers", "portRegIn0", "8,0", "black", "Rs1");
            CreateConnector(IFIDToRegIn1, "IFID", "portIfidOut1", "Registers", "portRegIn1", "8,0", "black", "Rs2");
            CreateConnector(RegToIDEXIn1, "Registers", "portRegOut0", "IDEX", "portIdexIn1", "8,0", "black", "RsD1", AnnotationAlignment.Before);
            CreateConnector(RegToIDEXIn2, "Registers", "portRegOut1", "IDEX", "portIdexIn2", "8,0", "black", "RsD2", AnnotationAlignment.Before);
            CreateConnector(IFIDToImmGen, "IFID", "portIfidOut1", "ImmGen", "portImmGenIn", "8,0", "black", "Imm/Addr Mode");
            CreateConnector(ImmGenToIDEX, "ImmGen", "portImmGenOut", "IDEX", "portIdexIn3", "8,0", "black", "Result", AnnotationAlignment.Before);
            CreateConnector(IFIDToIDEX, "IFID", "portIfidOut1", "IDEX", "portIdexIn4", "8,0", "black", "Rd");
            CreateConnector(IFIDToControl, "IFID", "portIfidOut1", "Control", "portControlIn", "8,0", "black", "Control Bits", AnnotationAlignment.Center, .85);
            CreateConnector(ControlToRW, "Control", "portControlOut1", "RW", "portRWIn", ControlDash, "black");
            CreateConnector(ControlToMTR, "Control", "portControlOut1", "MTR", "portMTRIn", ControlDash, "black");
            CreateConnector(ControlToMR, "Control", "portControlOut1", "MR", "portMRIn", ControlDash, "black");
            CreateConnector(ControlToMW, "Control", "portControlOut1", "MW", "portMWIn", ControlDash, "black");
            CreateConnector(ControlToPCS, "Control", "portControlOut1", "PCS", "portPCSIn", ControlDash, "black");
            CreateConnector(ControlToALUS, "Control", "portControlOut1", "ALUS", "portALUSIn", ControlDash, "black");

            // Execute Connectors
            CreateConnector(RWToRW1, "RW", "portRWOut", "RW1", "portRW1In", ControlDash, "black");
            CreateConnector(MTRToMTR1, "MTR", "portMTROut", "MTR1", "portMTR1In", ControlDash, "black");
            CreateConnector(MRToMR1, "MR", "portMROut", "MR1", "portMR1In", ControlDash, "black");
            CreateConnector(MWToMW1, "MW", "portMWOut", "MW1", "portMW1In", ControlDash, "black");
            CreateConnector(PCSToPCS1, "PCS", "portPCSOut", "PCS1", "portPCS1In", ControlDash, "black");
            CreateConnector(ALUSToExecuteMux, "ALUS", "portALUSOut", "ExecuteMux", "portExecuteMuxIn2", ControlDash, "black");
            idExRsD2 = new PathAnnotation
            {
                Content = "null",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Alignment = AnnotationAlignment.Center,
                Offset = -0.26,
                Style = new TextStyle() {Color = "red", FontSize = 9.0}
            };
            CreateConnector(IDEXToExecuteMux1, "IDEX", "portIdexOut2", "ExecuteMux", "portExecuteMuxIn1", "8,0", "black", "0", AnnotationAlignment.Center, .7, idExRsD2);
            CreateConnector(IDEXToEXMEM2, "IDEX", "portIdexOut2", "EXMEM", "portExmemIn2", "8,0", "black", "RsD2", AnnotationAlignment.Center, .9);
            CreateConnector(IDEXToFetchMux, "IDEX", "portIdexOut3", "FetchMux", "portFetchMuxIn0", "8,0", "black", "1", AnnotationAlignment.Center, .65);
            idExImmediate = new PathAnnotation
            {
                Content = "null",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Alignment = AnnotationAlignment.Center,
                Offset = -0.14,
                Style = new TextStyle() {Color = "red", FontSize = 9.0 }
            };
            CreateConnector(IDEXToExecuteMux0, "IDEX", "portIdexOut3", "ExecuteMux", "portExecuteMuxIn0", "8,0",
                "black", "1", AnnotationAlignment.Before, .7, idExImmediate);
            idExRd = new PathAnnotation
            {
                Content = "null",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Alignment = AnnotationAlignment.Center,
                Offset = -0.06,
                Style = new TextStyle() {Color = "red", FontSize = 9.0 }
            };
            CreateConnector(IDEXToEXMEM3, "IDEX", "portIdexOut4", "EXMEM", "portExmemIn3", "8,0", "black", "Rd", AnnotationAlignment.Center, .9, idExRd);
            idExRsD1 = new PathAnnotation
            {
                Content = "null",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Alignment = AnnotationAlignment.Center,
                Offset = -0.08,
                Style = new TextStyle() {Color = "red", FontSize = 9.0 }
            };
            CreateConnector(IDEXToALU, "IDEX", "portIdexOut1", "ALU", "portALUIn1", "8,0", "black", "RsD1", AnnotationAlignment.Center, .5, idExRsD1);
            CreateConnector(ExecuteMuxToALU, "ExecuteMux", "portExecuteMuxOut0", "ALU", "portALUIn0", "8,0", "black");
            CreateConnector(ALUToEXMEM1, "ALU", "portALUOut0", "EXMEM", "portExmemIn1", "8,0", "black", "ALUr", AnnotationAlignment.After, 0);
            CreateConnector(ALUToEXMEM0, "ALU", "portALUOut1", "EXMEM", "portExmemIn0", "8,0", "black", "TMP FLGS", AnnotationAlignment.After, .74);

            // Memory Connectors
            CreateConnector(RW1ToRW2, "RW1", "portRW1Out", "RW2", "portRW2In", ControlDash, "black");
            CreateConnector(MTR1ToMTR2, "MTR1", "portMTR1Out", "MTR2", "portMTR2In", ControlDash, "black");
            CreateConnector(MR1ToDataMem, "MR1", "portMR1Out", "DataMem", "portDataMemIn3", ControlDash, "black");
            CreateConnector(MW1ToDataMem, "MW1", "portMW1Out", "DataMem", "portDataMemIn2", ControlDash, "black");
            CreateConnector(EXMEMToDataMem0, "EXMEM", "portExmemOut1", "DataMem", "portDataMemIn0", "8,0", "black", "Addr");
            exMemRsD2 = new PathAnnotation
            {
                Content = "null",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Alignment = AnnotationAlignment.Center,
                Offset = -0.075,
                Style = new TextStyle() {Color = "red", FontSize = 9.0 }
            };
            CreateConnector(EXMEMToDataMem1, "EXMEM", "portExmemOut2", "DataMem", "portDataMemIn1", "8,0", "black", "RsD2", AnnotationAlignment.Before, 1, exMemRsD2);
            exMemAluResult = new PathAnnotation
            {
                Content = "null",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Alignment = AnnotationAlignment.Center,
                Offset = -0.03,
                Style = new TextStyle() {Color = "red", FontSize = 9.0 }
            };
            CreateConnector(EXMEMToMEMWB1, "EXMEM", "portExmemOut1", "MEMWB", "portMemwbIn1", "8,0", "black", "ALUr", AnnotationAlignment.Before, 1, exMemAluResult);
            exMemRd = new PathAnnotation
            {
                Content = "null",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Alignment = AnnotationAlignment.Center,
                Offset = -0.06,
                Style = new TextStyle() {Color = "red", FontSize = 9.0 }
            };
            CreateConnector(EXMEMToMEMWB2, "EXMEM", "portExmemOut3", "MEMWB", "portMemwbIn2", "8,0", "black", "Rd", AnnotationAlignment.Before, 1, exMemRd);
            CreateConnector(DataMemToMEMWB, "DataMem", "portDataMemOut", "MEMWB", "portMemwbIn0", "8,0", "black", "Data");
            CreateConnector(PCS1ToCheckFlags, "PCS1", "portPCS1Out", "CheckFlags", "portChkFlgIn1", ControlDash, "black");
            CreateConnector(EXMEMToCheckFlags, "EXMEM", "portExmemOut0", "CheckFlags", "portChkFlgIn0", ControlDash, "black", "FL");
            CreateConnector(CheckFlagsToFlgRet, "CheckFlags", "portChkFlgOut0", "FlgReturn", "portFLRetIn", ControlDash, "black");
            CreateConnector(FlgRetToFetchMux, "FlgReturn", "portFLRetOut", "FetchMux", "portFetchMuxIn1", ControlDash, "black");

            // WriteBack Connectors
            CreateConnector(MTR2ToWriteMux, "MTR2", "portMTR2Out", "WriteMux", "portWriteMuxIn2", ControlDash, "black");
            CreateConnector(RW2ToRWRet, "RW2", "portRW2Out", "RWReturn", "portRWRetIn", ControlDash, "black");
            CreateConnector(RWRetToReg, "RWReturn", "portRWRetOut", "Registers", "portRegIn4", ControlDash, "black");
            memWbMemData = new PathAnnotation
            {
                Content ="null",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Alignment = AnnotationAlignment.Center,
                Offset = -0.12,
                Style = new TextStyle() {Color = "red", FontSize = 9.0 }
            };
            CreateConnector(MEMWBToWriteMux1, "MEMWB", "portMemwbOut0", "WriteMux", "portWriteMuxIn1", "8,0", "black", "1", AnnotationAlignment.Center, .5, memWbMemData);
            memWbAluResult = new PathAnnotation
            {
                Content = "null",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Alignment = AnnotationAlignment.Center,
                Offset = -0.075,
                Style = new TextStyle() {Color = "red", FontSize = 9.0 }
            };
            CreateConnector(MEMWBToWriteMux2, "MEMWB", "portMemwbOut1", "WriteMux", "portWriteMuxIn0", "8,0", "black", "0", AnnotationAlignment.Before, .8, memWbAluResult);
            CreateConnector(WriteMuxToReg, "WriteMux", "portWriteMuxOut0", "Registers", "portRegIn3", "8,0", "black", "Rd Data", AnnotationAlignment.After, .5, null, segment1, segment2);
            memWbRd = new PathAnnotation
            {
                Content = "null",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Alignment = AnnotationAlignment.Center,
                Offset = -0.03,
                Style = new TextStyle() {Color = "red", FontSize = 9.0 }
            };
            CreateConnector(MEMWBToRdRet, "MEMWB", "portMemwbOut2", "RdRegReturn", "portRdRetIn", "8,0", "black", "Rd Reg", AnnotationAlignment.Before, .5, memWbRd);
            CreateConnector(RdRetToReg, "RdRegReturn", "portRdRetOut", "Registers", "portRegIn2", "8,0", "black", "Rd Reg", AnnotationAlignment.Before, .5);

            #endregion

            UpdateDiagram();
        }

        #region Connector Variables

        // Fetch Connectors
        public string FetchMuxToPCIn = "FetchMuxToPCIn";
        public string PCToInstrMemIn = "PCToInstrMemIn";
        public string PCToAddPC = "PCToAddPC";
        public string AddPCToFetchMux = "AddPCToFetchMux";
        public string IntrMemToIFID = "IntrMemToIFID";
        public string IntrMemToAddPC = "IntrMemToAddPC";

        // Decode Connectors
        public string IFIDToRegIn0 = "IFIDToRegIn0";
        public string IFIDToRegIn1 = "IFIDToRegIn1";
        public string RegToIDEXIn1 = "RegToIDEXIn1";
        public string RegToIDEXIn2 = "RegToIDEXIn2";
        public string IFIDToImmGen = "IFIDToImmGen";
        public string ImmGenToIDEX = "ImmGenToIDEX";
        public string IFIDToIDEX = "IFIDToIDEX";
        public string IFIDToControl = "IFIDToControl";
        public string ControlToRW = "ControlToRW";
        public string ControlToMTR = "ControlToMTR";
        public string ControlToMR = "ControlToMR";
        public string ControlToMW = "ControlToMW";
        public string ControlToPCS = "ControlToPCS";
        public string ControlToALUS = "ControlToALUS";

        // Execute Connectors
        public string RWToRW1 = "RWToRW1";
        public string MTRToMTR1 = "MTRToMTR1";
        public string MRToMR1 = "MRToMR1";
        public string MWToMW1 = "MWToMW1";
        public string PCSToPCS1 = "PCSToPCS1";
        public string ALUSToExecuteMux = "ALUSToExecuteMux";
        public string IDEXToExecuteMux1 = "IDEXToExecuteMux1";
        public string IDEXToEXMEM2 = "IDEXToEXMEM2";
        public string IDEXToFetchMux = "IDEXToFetchMux";
        public string IDEXToExecuteMux0 = "IDEXToExecuteMux0";
        public string IDEXToEXMEM3 = "IDEXToEXMEM3";
        public string IDEXToALU = "IDEXToALU";
        public string ExecuteMuxToALU = "ExecuteMuxToALU";
        public string ALUToEXMEM1 = "ALUToEXMEM1";
        public string ALUToEXMEM0 = "ALUToEXMEM0";

        // Memory Connectors
        public string RW1ToRW2 = "RW1ToRW2";
        public string MTR1ToMTR2 = "MTR1ToMTR2";
        public string MR1ToDataMem = "MR1ToDataMem";
        public string MW1ToDataMem = "MW1ToDataMem";
        public string EXMEMToDataMem0 = "EXMEMToDataMem0";
        public string EXMEMToDataMem1 = "EXMEMToDataMem1";
        public string EXMEMToMEMWB1 = "EXMEMToMEMWB1";
        public string EXMEMToMEMWB2 = "EXMEMToMEMWB2";
        public string DataMemToMEMWB = "DataMemToMEMWB";
        public string PCS1ToCheckFlags = "PCS1ToCheckFlags";
        public string EXMEMToCheckFlags = "EXMEMToCheckFlags";
        public string CheckFlagsToFlgRet = "CheckFlagsToFlgRet";
        public string FlgRetToFetchMux = "FlgRetToFetchMux";

        // WriteBack Connectors
        public string MTR2ToWriteMux = "MTR2ToWriteMux";
        public string RW2ToRWRet = "RW2ToRWRet";
        public string RWRetToReg = "RWRetToReg";
        public string MEMWBToWriteMux1 = "MEMWBToWriteMux1";
        public string MEMWBToWriteMux2 = "MEMWBToWriteMux2";
        public string WriteMuxToReg = "WriteMuxToReg";
        public string MEMWBToRdRet = "MEMWBToRdRet";
        public string RdRetToReg = "RdRetToReg";

        #endregion

        public string RegistersRdReg = "r2";

        private void CreateConnector(string id, string sourceId, string sourcePortId, string targetId, string targetPortId, string strokeDash, string strokeColor, string label = default,
                                     AnnotationAlignment align = AnnotationAlignment.Before, double offset = 1, PathAnnotation pAnnotate = null, OrthogonalSegment segment1 = null, OrthogonalSegment segment2 = null)
        {
            Connector diagramConnector = new Connector()
            {
                ID = id,
                SourceID = sourceId,
                SourcePortID = sourcePortId,
                TargetID = targetId,
                TargetPortID = targetPortId,
                Style = new ShapeStyle() { StrokeWidth = 1, StrokeColor = strokeColor, StrokeDashArray = strokeDash },
                TargetDecorator = new DecoratorSettings()
                {
                    Style = new ShapeStyle() { StrokeColor = strokeColor, Fill = strokeColor }
                }
            };
            diagramConnector.Constraints |= ConnectorConstraints.DragSegmentThumb;
            diagramConnector.Type = DiagramSegments.Orthogonal;
            if (segment1 != null)
            {
                diagramConnector.Segments = new DiagramObjectCollection<ConnectorSegment>() { segment1, segment2 };
            }
            if (label != default(string))
            {
                var annotation = new PathAnnotation()
                {
                    Content = label,
                    Style = new TextStyle() { Fill = "transparent" },
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Alignment = align
                };
                
                annotation.Offset = offset;

                if(pAnnotate != null)
                {
                    diagramConnector.Annotations = new DiagramObjectCollection<PathAnnotation>() { annotation, pAnnotate };
                }
                else
                {
                    diagramConnector.Annotations = new DiagramObjectCollection<PathAnnotation>() { annotation };
                }
            }

            ConnectorCollection.Add(diagramConnector);
        }

        private PointPort AddPort(string id, double x, double y)
        {
            return new PointPort()
            {
                ID = id,
                Shape = PortShapes.Circle,
                Width = 0,
                Height = 0,
                Visibility = PortVisibility.Visible,
                Offset = new DiagramPoint() { X = x, Y = y },
                Style = new ShapeStyle() { Fill = "#1916C1", StrokeColor = "#000" },
                Constraints = PortConstraints.Default | PortConstraints.Draw
            };
        }

        private void CreateNode(string id, double xOffset, double yOffset, int xSize, int ySize, int rAngleNode, int rAngleAnnotation,
            List<PointPort> ports, FlowShapeType shape, string label, string fillColor, string strokeDash, string stroke, string? labelColor = default, 
            DiagramPoint diagramPoint = null, ShapeAnnotation sAnnotate1 = null)
        {
            ShapeAnnotation annotation = new ShapeAnnotation()
            {
                Content = label,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                RotationAngle = rAngleAnnotation
            };
            if (diagramPoint != null)
                annotation.Offset = diagramPoint;
            annotation.Style = new TextStyle()
            {
                Color = labelColor != default ? labelColor : "black",
                Fill = "transparent"
            };
            Node diagramNode = new Node()
            {
                ID = id,
                OffsetX = xOffset,
                OffsetY = yOffset,
                Width = xSize,
                Height = ySize,
                RotationAngle = rAngleNode,
                Shape = new FlowShape() { Type = Shapes.Flow, Shape = shape },
                Style = new ShapeStyle() { Fill = fillColor, StrokeColor = stroke, StrokeDashArray = strokeDash },
                Annotations = sAnnotate1 != null ? new DiagramObjectCollection<ShapeAnnotation>() { annotation, sAnnotate1 } : new DiagramObjectCollection<ShapeAnnotation>() { annotation },
                Ports = new DiagramObjectCollection<PointPort>(ports)
            };
            if (diagramNode.ID.ToString() == "Ready")
            {
                diagramNode.Height = 100;
                diagramNode.Width = 140;
            }
            NodeCollection.Add(diagramNode);
        }

        private void CreateNode(string id, double xOffset, double yOffset, int xSize, int ySize, int rAngleNode, int rAngleAnnotation,
            List<PointPort> ports, BasicShapeType shape, string label, string fillColor, string strokeDash, string stroke, string labelColor = default)
        {
            ShapeAnnotation annotation = new ShapeAnnotation()
            {
                Content = label,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                RotationAngle = rAngleAnnotation
            };
            annotation.Style = new TextStyle()
            {
                Color = labelColor != default ? labelColor : "black",
                Fill = "transparent"
            };
            Node diagramNode = new Node()
            {
                ID = id,
                OffsetX = xOffset,
                OffsetY = yOffset,
                Width = xSize,
                Height = ySize,
                RotationAngle = rAngleNode,
                Shape = new BasicShape() { Type = Shapes.Basic, Shape = shape },
                Style = new ShapeStyle() { Fill = fillColor, StrokeColor = stroke, StrokeDashArray = strokeDash },
                Annotations = new DiagramObjectCollection<ShapeAnnotation>() { annotation },
                Ports = new DiagramObjectCollection<PointPort>(ports)
            };
            if (diagramNode.ID.ToString() == "Ready")
            {
                diagramNode.Height = 100;
                diagramNode.Width = 140;
            }
            NodeCollection.Add(diagramNode);
        }

        public void OnItemClick()
        {
            ShowItem = !ShowItem;
        }

        private string diagramContent = "";

        private void DialogOpen(Object args)
        {
            debugRender = true;
            this.ShowButton = true;
            debugRender = true;
        }
        private void DialogClose(Object args)
        {
            debugRender = true;
            diagramContent = diagram.SaveDiagram();
            this.ShowButton = false;
            debugRender = true;
        }
        private void OnClicked()
        {
            InitDiagramModel();
            this.Visibility = true;
        }

        private void errorClose(Object args)
        {
            this.errorVis = false;
        }

    }
}