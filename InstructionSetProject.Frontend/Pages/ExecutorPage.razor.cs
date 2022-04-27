using System.Collections.Specialized;
using System.Text;
using BlazorMonaco;
using InstructionSetProject.Backend;
using InstructionSetProject.Backend.DynamicPipeline;
using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.StaticFrontend;
using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Diagram;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using DiagramSegments = Syncfusion.Blazor.Diagram.ConnectorSegmentType;

namespace InstructionSetProject.Frontend.Pages
{
    public partial class ExecutorPage
    {
        ElementReference SyntaxCode;
        private MonacoEditor _editor { get; set; }

        private string ExecMachineCode = "";
        private string ExecAssemblyCode = "";
        private string statsString = "";
        private string space = " ";
        private string newLine = "\n";
        private int charCount = 0;
        private int progCount = 0;
        private int progCharCount = 0;
        bool isGranted = true;
        bool StaticMode = true;
        int sliderValue = 0;
        List<char> memoryCharList = new List<char>();

        public List<CacheLineData> L1CacheSource
        {
            get
            {
                var cache = StaticMode ? SPEx != null ? SPEx.DataStructures.L1 : null
                    : DPEx != null ? DPEx.dataStructures.L1 : null;
                if (cache == null) return new List<CacheLineData>();
                var data = new List<CacheLineData>();
                
                for (int i = 0; i < cache.Sets.Count; i++)
                {
                    var set = cache.Sets[i];
                    for (int j = 0; j < set.Lines.Count; j++)
                    {
                        var line = set.Lines.ToArray()[j].Key;
                        var offsetSize = Math.ILogB(cache.Config.LineSize);
                        var indexSize = Math.ILogB(cache.Config.SetCount);
                        var offsetMask = (1 << offsetSize) - 1;
                        var indexMask = (1 << indexSize) - 1;
                        if (line.Address == null)
                        {
                            data.Add(new CacheLineData("0x" + i.ToString("X2"), "0x" + 0.ToString("X2"), 0.ToString("X2")));
                            continue;
                        }
                        var offset = (int)line.Address & offsetMask;
                        var index = (int)(line.Address >> offsetSize) & indexMask;
                        var tag = (int) (line.Address >> (offsetSize + indexSize));
                        var lineData = String.Join(" ", line.Data.Select(entry => entry.ToString("X2")));
                        data.Add(new CacheLineData("0x" + index.ToString("X2"), "0x" + tag.ToString("X2"), lineData));                        
                    }
                }

                return data;
            }
        }

        public List<CacheLineData> L2CacheSource
        {
            get
            {
                var cache = StaticMode ? SPEx != null ? SPEx.DataStructures.L2 : null
                    : DPEx != null ? DPEx.dataStructures.L2 : null;
                if (cache == null) return new List<CacheLineData>();
                var data = new List<CacheLineData>();
                
                for (int i = 0; i < cache.Sets.Count; i++)
                {
                    var set = cache.Sets[i];
                    for (int j = 0; j < set.Lines.Count; j++)
                    {
                        var line = set.Lines.ToArray()[j].Key;
                        var offsetSize = Math.ILogB(cache.Config.LineSize);
                        var indexSize = Math.ILogB(cache.Config.SetCount);
                        var offsetMask = (1 << offsetSize) - 1;
                        var indexMask = (1 << indexSize) - 1;
                        if (line.Address == null)
                        {
                            data.Add(new CacheLineData("0x" + i.ToString("X2"), "0x" + 0.ToString("X2"), 0.ToString("X2")));
                            continue;
                        }
                        var offset = (int)line.Address & offsetMask;
                        var index = (int)(line.Address >> offsetSize) & indexMask;
                        var tag = (int) (line.Address >> (offsetSize + indexSize));
                        var lineData = String.Join(" ", line.Data.Select(entry => entry.ToString("X2")));
                        data.Add(new CacheLineData("0x" + index.ToString("X2"), "0x" + tag.ToString("X2"), lineData));                        
                    }
                }

                return data;
            }
        }

        public string[] GroupedColumns = new string[] { "Index" };

        protected override bool ShouldRender()
        {
            return true;
        }

        bool darkModeExecutorPage = FrontendVariables.darkMode;
        public string MemDumpStart { get; set; } = "";

        public string MemDumpContent => StaticMode ? SPEx != null
            ? String.Join("",
                SPEx.DataStructures.Memory
                    .GetBytesAtAddress(MemDumpStart != string.Empty ? Convert.ToUInt32(MemDumpStart, 16) : 0)
                    .Select((memByte) => memByte.ToString("X2")))
            : ""
            :
            DPEx != null
                ? String.Join("",
                    DPEx.dataStructures.Memory
                        .GetBytesAtAddress(MemDumpStart != string.Empty ? Convert.ToUInt32(MemDumpStart, 16) : 0)
                        .Select((memByte) => memByte.ToString("X2")))
                : ""
            ;

        private StaticPipelineExecution? SPEx;
        private DynamicPipelineExecution? DPEx;

        public byte[]? MemoryBytes => SPEx != null ? SPEx.DataStructures.Memory.Bytes : null;

        public string r0 => StaticMode ? SPEx != null ? SPEx.DataStructures.R0.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.R0.value.ToString("X4") : "0000";
        public string r1 => StaticMode ? SPEx != null ? SPEx.DataStructures.R1.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.R1.value.ToString("X4") : "0000";
        public string r2 => StaticMode ? SPEx != null ? SPEx.DataStructures.R2.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.R2.value.ToString("X4") : "0000";
        public string r3 => StaticMode ? SPEx != null ? SPEx.DataStructures.R3.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.R3.value.ToString("X4") : "0000";
        public string r4 => StaticMode ? SPEx != null ? SPEx.DataStructures.R4.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.R4.value.ToString("X4") : "0000";
        public string r5 => StaticMode ? SPEx != null ? SPEx.DataStructures.R5.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.R5.value.ToString("X4") : "0000";
        public string r6 => StaticMode ? SPEx != null ? SPEx.DataStructures.R6.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.R6.value.ToString("X4") : "0000";
        public string r7 => StaticMode ? SPEx != null ? SPEx.DataStructures.R7.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.R7.value.ToString("X4") : "0000";
        public string f0 => StaticMode ? SPEx != null ? SPEx.DataStructures.F0.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.F0.value.ToString("X4") : "0000";
        public string f1 => StaticMode ? SPEx != null ? SPEx.DataStructures.F1.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.F1.value.ToString("X4") : "0000";
        public string f2 => StaticMode ? SPEx != null ? SPEx.DataStructures.F2.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.F2.value.ToString("X4") : "0000";
        public string f3 => StaticMode ? SPEx != null ? SPEx.DataStructures.F3.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.F3.value.ToString("X4") : "0000";
        public string f4 => StaticMode ? SPEx != null ? SPEx.DataStructures.F4.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.F4.value.ToString("X4") : "0000";
        public string f5 => StaticMode ? SPEx != null ? SPEx.DataStructures.F5.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.F5.value.ToString("X4") : "0000";
        public string f6 => StaticMode ? SPEx != null ? SPEx.DataStructures.F6.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.F6.value.ToString("X4") : "0000";
        public string f7 => StaticMode ? SPEx != null ? SPEx.DataStructures.F7.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.F7.value.ToString("X4") : "0000";
        public string IP => StaticMode ? SPEx != null ? SPEx.DataStructures.InstructionPointer.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.InstructionPointer.value.ToString("X4") : "0000";
        public string SP => StaticMode ? SPEx != null ? SPEx.DataStructures.StackPointer.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.StackPointer.value.ToString("X4") : "0000";
        public string FL => StaticMode ? SPEx != null ? SPEx.DataStructures.Flags.AsRegisterValue().ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.Flags.AsRegisterValue().ToString("X4") : "0000";
        public string PC => StaticMode ? SPEx != null ? SPEx.DataStructures.InstructionPointer.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.InstructionPointer.value.ToString("X4") : "0000";
        public string MBP => StaticMode ? SPEx != null ? SPEx.DataStructures.MemoryBasePointer.value.ToString("X4") : "0000" : DPEx != null ? DPEx.dataStructures.MemoryBasePointer.value.ToString("X4") : "0000";

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
        private bool StaticVisibility { get; set; } = false;
        private bool DynamicVisibility { get; set; } = false;
        private bool errorVis { get; set; } = false;
        private bool ShowButton { get; set; } = false;
        private bool ShowCache { get; set; } = false;
        private string SelectedCache { get; set; } = "L1";
        private SfGrid<CacheLineData>? L1CacheGridObject;
        private SfGrid<CacheLineData>? L2CacheGridObject;
        private bool L1CacheExpandToggle = false;
        private bool L2CacheExpandToggle = false;
        private ResizeDirection[] dialogResizeDirections { get; set; } = new ResizeDirection[] { ResizeDirection.All };

        public bool IsModalOpened { get; set; }
        bool visible { get; set; } = false;

        bool memoryDumpSwitch { get; set; } = false;

        void toggleVisible()
        {
            visible = !visible;
        }

        void OnClose(string value)
        {

        }

        void toggleStaticDynamicMode()
        {
            StaticMode = !StaticMode;
            ConnectorCollection.Clear();
            NodeCollection.Clear();
        }

        void toggleMemoryDump()
        {
            memoryDumpSwitch = !memoryDumpSwitch;
        }

        private async Task LoadFile(InputFileChangeEventArgs e)
        {
            var file = e.File;
            long maxsize = 512000;

            var buffer = new byte[file.Size];
            await file.OpenReadStream(maxsize).ReadAsync(buffer);
            fileContent = System.Text.Encoding.UTF8.GetString(buffer);
            await _editor.SetValue(fileContent);
        }

        private async Task SaveAssemblyCode()
        {
            byte[] file = System.Text.Encoding.UTF8.GetBytes(await _editor.GetValue());
            await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", "assemblyCode.txt", "text/plain", file);
        }
        private async Task SaveStats()
        {
            byte[] file = System.Text.Encoding.UTF8.GetBytes(statsString);
            await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", "assemblyStats.txt", "text/plain", file);
        }

        void addToMemoryCharList(char character)
        {
            memoryCharList.Add(character);
        }

        void clearMemoryCharList()
        {
            memoryCharList.Clear();
        }

        string printMemoryCharList()
        {
            string ascii = " : ";

            ascii += ConvertHex(String.Join("", memoryCharList));

            return ascii;
        }

        public static string ConvertHex(String hexString)
        {
            try
            {
                string ascii = string.Empty;

                for (int i = 0; i < hexString.Length; i += 2)
                {
                    String hs = string.Empty;

                    hs = hexString.Substring(i, 2);
                    uint decval = System.Convert.ToUInt32(hs, 16);
                    char character = (decval > 126 || decval < 32) ? '.' : System.Convert.ToChar(decval);
                    ascii += character;

                }

                return ascii;
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.Message); 
            }

            return string.Empty;
        }

        string PrintProgCount()
        {
            return progCount.ToString("X4") + "  :  ";
        }

        char print0()
        {
            return '0';
        }

        void UpdateCounters()
        {
            ClearProgCount();
            ClearProgCharCount();
            clearMemoryCharList();
            setProgCounter();
        }

        void setProgCounter()
        {
            if (MemDumpStart != "")
            {
                progCount = (int) Convert.ToUInt32(MemDumpStart, 16);
            }
        }

        void IncrementProgCount()
        {
            progCount+=16;
        }

        void ClearProgCount()
        {
            progCount = 0;
        }

        void IncrementProgCharCount()
        {
            progCharCount++;
        }

        void ClearProgCharCount()
        {
            progCharCount = 0;
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
            if (StaticMode && SPEx == null) return "No Statistics Yet";
            if (!StaticMode && DPEx == null) return "No Statistics Yet";
            var stats = StaticMode ? SPEx.Statistics : DPEx.statistics;
            statsString += "Instruction Types\n";
            statsString += "-----------------\n";
            statsString += "R2 Type: " + stats.R2InstructionCount + "\n";
            statsString += "R3 Type: " + stats.R3InstructionCount + "\n";
            statsString += "Rm Type: " + stats.RmInstructionCount + "\n";
            statsString += "Rs Type: " + stats.RsInstructionCount + "\n";
            statsString += "F2 Type: " + stats.F2InstructionCount + "\n";
            statsString += "F3 Type: " + stats.F3InstructionCount + "\n";
            statsString += "Fm Type: " + stats.FmInstructionCount + "\n\n";

            statsString += "Clock\n";
            statsString += "-----\n";
            statsString += "Total Clock Ticks: " + stats.ClockTicks + "\n\n";

            statsString += "Flush\n";
            statsString += "-----\n";
            statsString += "Total Flushes: " + stats.FlushCount;

            return statsString;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            UpdateCounters();
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
            if (firstRender)
            {
                await _editor.SetValue("");
            }
            debugRender = false;
        }

        async Task ChangedCode()
        {
            await JSRuntime.InvokeVoidAsync("handleKeyPress", SyntaxCode);
        }

        protected override Task OnInitializedAsync()
        {
            StartupMethod();
            Statistics();
            return Task.CompletedTask;
        }

        void StartupMethod()
        {
            ExecMachineCode = FrontendVariables.currentMachineCodeExecutor;
            FrontendVariables.currentMachineCodeExecutor = "";
            ExecAssemblyCode = FrontendVariables.currentAssemblyCodeExecutor;
            FrontendVariables.currentAssemblyCodeExecutor = "";
        }

        async Task buildCodeAsync()
        {
            if (!_editor.GetValue().Equals(""))
            {
                try
                {
                    machineCode = Assembler.Assemble(await _editor.GetValue());
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
            ClearProgCount();
            ClearProgCharCount();
            clearMemoryCharList();
        }

        async Task runCodeAsync()
        {
            var l1Config = new CacheConfiguration(1, 8, 8, CacheEvictionStrategy.LRU, CacheWriteStrategy.WriteBack);
            var l2Config = new CacheConfiguration(4, 8, 64, CacheEvictionStrategy.LRU, CacheWriteStrategy.WriteBack);
            try
            {
                if (StaticMode)
                    SPEx = StaticPipelineExecutor.Execute(await _editor.GetValue(), l1Config, l2Config);
                else
                    DPEx = DynamicPipelineExecutor.Execute(await _editor.GetValue(), l1Config, l2Config);

                output = "";
            }
            catch (Exception ex)
            {
                output = "ERROR: " + ex.Message + "\n";
            }

            debugRender = true;
            try
            {
                if (StaticMode)
                    SPEx.Continue();
                else
                    DPEx.Continue();
                output = "";
            }
            catch (Exception ex)
            {
                output = "ERROR: " + ex.Message + "\n";
            }
            debugRender = true;
            Statistics();
            ClearProgCount();
            ClearProgCharCount();
            clearMemoryCharList();
        }

        async Task Debug()
        {
            var l1Config = new CacheConfiguration(1, 8, 8, CacheEvictionStrategy.LRU, CacheWriteStrategy.WriteBack);
            var l2Config = new CacheConfiguration(4, 8, 64, CacheEvictionStrategy.LRU, CacheWriteStrategy.WriteBack);

            try
            {
                if (StaticMode)
                    SPEx = StaticPipelineExecutor.Execute(await _editor.GetValue(), l1Config, l2Config);
                else
                    DPEx = DynamicPipelineExecutor.Execute(await _editor.GetValue(), l1Config, l2Config);
                output = "";
            }
            catch (Exception ex)
            {
                output = "ERROR: " + ex.Message + "\n";
            }
            OnItemClick();
            debugRender = true;
            _ = JSRuntime.InvokeVoidAsync("debugScrollToTop");
            if (StaticMode == true)
                UpdateStaticDiagram();
            else if (StaticMode == false)
                UpdateDynamicDiagram();
            ClearProgCount();
            ClearProgCharCount();
            clearMemoryCharList();
        }

        bool IsSelectedFetch(IInstruction instr) => StaticMode ? instr == SPEx.fetchingInstruction : DPEx.instrQueue.nextBatch.Exists((search) => search.instruction == instr);
        bool IsSelectedDecode(IInstruction instr) => StaticMode ? instr == SPEx.decodingInstruction : 
            (
                DPEx.integerUnit?.instruction == instr ||
                DPEx.integerReservationStation.instructions.FirstOrDefault((search) => search.instruction == instr)?.instruction == instr ||
                DPEx.fpAdder?.instruction == instr ||
                DPEx.fpAdderReservationStation.instructions.FirstOrDefault((search) => search.instruction == instr)?.instruction == instr ||
                DPEx.fpMul?.instruction == instr ||
                DPEx.fpMulReservationStation.instructions.FirstOrDefault((search) => search.instruction == instr)?.instruction == instr
            );
        bool IsSelectedExecute(IInstruction instr) => StaticMode ? instr == SPEx.executingInstruction : 
            (
                DPEx.memoryUnit.activeInstruction?.instruction == instr ||
                DPEx.memoryUnit.loadBuffers.FirstOrDefault((search) => search.instruction == instr)?.instruction == instr
            );
        bool IsSelectedMemory(IInstruction instr) => StaticMode ? instr == SPEx.memoryInstruction : DPEx.commonDataBus.Exists((search) => search.instruction == instr);
        bool IsSelectedWrite(IInstruction instr) => StaticMode ? instr == SPEx.writingBackInstruction : DPEx.reorderBuffer.buffers.UnorderedItems.FirstOrDefault((search) => search.Element.instruction == instr).Element?.instruction == instr;

        string DivCSS(IInstruction instr) => IsSelectedFetch(instr) ? "bg-fetch text-white" : (IsSelectedDecode(instr) ? "bg-decode text-white" : (IsSelectedExecute(instr) ? "bg-execute text-white" : (IsSelectedMemory(instr) ? "bg-memory text-white" : (IsSelectedWrite(instr) ? "bg-write text-white" : (FrontendVariables.darkMode ? "bg-dark-mode" : "bg-white")))));

        async Task ClockPress()
        {
            if (sliderValue != 0)
            {
                while (sliderValue != 0 && (SPEx != null ? !SPEx.IsExecutionFinished() : false || DPEx != null ? !DPEx.IsExecutionFinished() : false))
                {
                    StateHasChanged();
                    await Task.Delay(sliderValue);
                    ClockTick();
                }
            }
            else
            {
                ClockTick();
            }
        }

        void ClockTick()
        {
            debugRender = true;
            try
            {
                if (StaticMode)
                {
                    SPEx.ClockTick();
                }
                else
                {
                    DPEx.ClockTick();
                }
                output = "";
            }
            catch (Exception ex)
            {
                output = "ERROR: " + ex.Message + "\n";
            }
            //JSRuntime.InvokeVoidAsync("stepScroll");
            Statistics();
            debugRender = true;
            if (StaticMode == true)
                UpdateStaticDiagram();
            else if (StaticMode == false)
                UpdateDynamicDiagram();
            ClearProgCount();
            ClearProgCharCount();
            clearMemoryCharList();

        }

        void step()
        {
            debugRender = true;
            try
            {
                if (StaticMode)
                    SPEx.Step();
                else
                    DPEx.Step();
                output = "";
            }
            catch (Exception ex)
            {
                output = "ERROR: " + ex.Message + "\n";
            }
            //JSRuntime.InvokeVoidAsync("stepScroll");
            Statistics();
            debugRender = true;
            if (StaticMode == true)
                UpdateStaticDiagram();
            else if (StaticMode == false)
                UpdateDynamicDiagram();
            ClearProgCount();
            ClearProgCharCount();
            clearMemoryCharList();
        }

        void Continue()
        {
            debugRender = true;
            try
            {
                if (StaticMode)
                {
                    SPEx.Continue();
                }
                else
                {
                    DPEx.Continue();
                }
                output = "";
            }
            catch (Exception ex)
            {
                output = "ERROR: " + ex.Message + "\n";
            }
            Statistics();
            ClearProgCount();
            ClearProgCharCount();
            clearMemoryCharList();
            debugRender = true;
        }

        void Stop()
        {
            SPEx = null;
            DPEx = null;
            if (StaticMode == true)
            {
                InitStaticDiagramModel();
            }
            else if (StaticMode == false)
            {
                InitDynamicDiagramModel();
            }
            ClearProgCount();
            ClearProgCharCount();
            clearMemoryCharList();
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

        #region Dynamic pipeline variables

        private string reorderBuffer1 = "ReorderBuffer1";
        private string reorderBuffer2 = "ReorderBuffer2";
        private string reorderBuffer3 = "ReorderBuffer3";
        private string reorderBuffer4 = "ReorderBuffer4";
        private string reorderBuffer5 = "ReorderBuffer5";
        private string instrQueue1 = "InstrQueue1";
        private string instrQueue2 = "InstrQueue2";
        private string instrQueue3 = "InstrQueue3";
        private string instrQueue4 = "InstrQueue4";
        private string instrQueue5 = "InstrQueue5";
        private string loadBuffer1 = "LoadBuffer1";
        private string loadBuffer2 = "LoadBuffer2";
        private string loadBuffer3 = "LoadBuffer3";
        private string loadBuffer4 = "LoadBuffer4";
        private string loadBuffer5 = "LoadBuffer5";
        private string fpAdder1 = "Res1DataBus1";
        private string fpAdder2 = "Res1DataBus2";
        private string fpAdder3 = "Res1DataBus3";
        private string fpMul1 = "Res2DataBus1";
        private string fpMul2 = "Res2DataBus2";
        private string fpMul3 = "Res2DataBus3";
        private string integer1 = "Res3DataBus1";
        private string integer2 = "Res3DataBus2";
        private string integer3 = "Res3DataBus3";
        private string fpAdderVal1 = "Res1OperandBus1";
        private string fpAdderVal2 = "Res1OperandBus2";
        private string fpAdderVal3 = "Res1OperandBus3";
        private string fpMulVal1 = "Res2OperandBus1";
        private string fpMulVal2 = "Res2OperandBus2";
        private string fpMulVal3 = "Res2OperandBus3";
        private string integerVal1 = "Res3OperandBus1";
        private string integerVal2 = "Res3OperandBus2";
        private string integerVal3 = "Res3OperandBus3";

        #endregion
        void UpdateDynamicDiagram()
        {
            if (DPEx == null || NodeCollection.Count == 0) return;
            UpdateInstructionQueue();
            UpdateLoadBuffers();
            UpdateReservationStations();
            UpdateReorderBuffer();
        }

        void UpdateInstructionQueue()
        {
            var firstInstr = DPEx.instrQueue.nextBatch.ElementAtOrDefault(0);
            if (firstInstr != null)
                GetNodeByID(instrQueue5).Annotations[0].Content = firstInstr.instruction.Disassemble();
            else
                GetNodeByID(instrQueue5).Annotations[0].Content = "";

            var secondInstr = DPEx.instrQueue.nextBatch.ElementAtOrDefault(1);
            if (secondInstr != null)
                GetNodeByID(instrQueue4).Annotations[0].Content = secondInstr.instruction.Disassemble();
            else
                GetNodeByID(instrQueue4).Annotations[0].Content = "";

            var thirdInstr = DPEx.instrQueue.nextBatch.ElementAtOrDefault(2);
            if (thirdInstr != null)
                GetNodeByID(instrQueue3).Annotations[0].Content = thirdInstr.instruction.Disassemble();
            else
                GetNodeByID(instrQueue3).Annotations[0].Content = "";

            var fourthInstr = DPEx.instrQueue.nextBatch.ElementAtOrDefault(3);
            if (fourthInstr != null)
                GetNodeByID(instrQueue2).Annotations[0].Content = fourthInstr.instruction.Disassemble();
            else
                GetNodeByID(instrQueue2).Annotations[0].Content = "";

            var fifthInstr = DPEx.instrQueue.nextBatch.ElementAtOrDefault(4);
            if (fifthInstr != null)
                GetNodeByID(instrQueue1).Annotations[0].Content = fifthInstr.instruction.Disassemble();
            else
                GetNodeByID(instrQueue1).Annotations[0].Content = "";
        }

        void UpdateLoadBuffers()
        {
            var firstInstr = DPEx.memoryUnit.loadBuffers.ElementAtOrDefault(0);
            if (firstInstr != null)
                GetNodeByID(loadBuffer5).Annotations[0].Content = firstInstr.instruction.GetMnemonic();
            else
                GetNodeByID(loadBuffer5).Annotations[0].Content = "";

            var secondInstr = DPEx.memoryUnit.loadBuffers.ElementAtOrDefault(1);
            if (secondInstr != null)
                GetNodeByID(loadBuffer4).Annotations[0].Content = secondInstr.instruction.GetMnemonic();
            else
                GetNodeByID(loadBuffer4).Annotations[0].Content = "";

            var thirdInstr = DPEx.memoryUnit.loadBuffers.ElementAtOrDefault(2);
            if (thirdInstr != null)
                GetNodeByID(loadBuffer3).Annotations[0].Content = thirdInstr.instruction.GetMnemonic();
            else
                GetNodeByID(loadBuffer3).Annotations[0].Content = "";

            var fourthInstr = DPEx.memoryUnit.loadBuffers.ElementAtOrDefault(3);
            if (fourthInstr != null)
                GetNodeByID(loadBuffer2).Annotations[0].Content = fourthInstr.instruction.GetMnemonic();
            else
                GetNodeByID(loadBuffer2).Annotations[0].Content = "";

            var fifthInstr = DPEx.memoryUnit.loadBuffers.ElementAtOrDefault(4);
            if (fifthInstr != null)
                GetNodeByID(loadBuffer1).Annotations[0].Content = fifthInstr.instruction.GetMnemonic();
            else
                GetNodeByID(loadBuffer1).Annotations[0].Content = "";
        }

        void UpdateReservationStations()
        {
            var fpAddInstr1 = DPEx.fpAdderReservationStation.instructions.ElementAtOrDefault(0);
            if (fpAddInstr1 != null)
            {
                GetNodeByID(fpAdder3).Annotations[0].Content = fpAddInstr1.instruction.GetMnemonic();
                GetNodeByID(fpAdderVal3).Annotations[0].Content =
                    fpAddInstr1.StillHasDependencies() ? "wait" : "ready";
            }
            else
            {
                GetNodeByID(fpAdder3).Annotations[0].Content = "";
                GetNodeByID(fpAdderVal3).Annotations[0].Content = "";
            }

            var fpAddInstr2 = DPEx.fpAdderReservationStation.instructions.ElementAtOrDefault(1);
            if (fpAddInstr2 != null)
            {
                GetNodeByID(fpAdder2).Annotations[0].Content = fpAddInstr2.instruction.GetMnemonic();
                GetNodeByID(fpAdderVal2).Annotations[0].Content =
                    fpAddInstr2.StillHasDependencies() ? "wait" : "ready";
            }
            else
            {
                GetNodeByID(fpAdder2).Annotations[0].Content = "";
                GetNodeByID(fpAdderVal2).Annotations[0].Content = "";
            }

            var fpAddInstr3 = DPEx.fpAdderReservationStation.instructions.ElementAtOrDefault(2);
            if (fpAddInstr3 != null)
            {
                GetNodeByID(fpAdder1).Annotations[0].Content = fpAddInstr3.instruction.GetMnemonic();
                GetNodeByID(fpAdderVal1).Annotations[0].Content =
                    fpAddInstr3.StillHasDependencies() ? "wait" : "ready";
            }
            else
            {
                GetNodeByID(fpAdder1).Annotations[0].Content = "";
                GetNodeByID(fpAdderVal1).Annotations[0].Content = "";
            }

            var fpMulInstr1 = DPEx.fpMulReservationStation.instructions.ElementAtOrDefault(0);
            if (fpMulInstr1 != null)
            {
                GetNodeByID(fpMul3).Annotations[0].Content = fpMulInstr1.instruction.GetMnemonic();
                GetNodeByID(fpMulVal3).Annotations[0].Content =
                    fpMulInstr1.StillHasDependencies() ? "wait" : "ready";
            }
            else
            {
                GetNodeByID(fpMul3).Annotations[0].Content = "";
                GetNodeByID(fpMulVal3).Annotations[0].Content = "";
            }

            var fpMulInstr2 = DPEx.fpMulReservationStation.instructions.ElementAtOrDefault(1);
            if (fpMulInstr2 != null)
            {
                GetNodeByID(fpMul2).Annotations[0].Content = fpMulInstr2.instruction.GetMnemonic();
                GetNodeByID(fpMulVal2).Annotations[0].Content =
                    fpMulInstr2.StillHasDependencies() ? "wait" : "ready";
            }
            else
            {
                GetNodeByID(fpMul2).Annotations[0].Content = "";
                GetNodeByID(fpMulVal2).Annotations[0].Content = "";
            }

            var fpMulInstr3 = DPEx.fpMulReservationStation.instructions.ElementAtOrDefault(2);
            if (fpMulInstr3 != null)
            {
                GetNodeByID(fpMul1).Annotations[0].Content = fpMulInstr3.instruction.GetMnemonic();
                GetNodeByID(fpMulVal1).Annotations[0].Content =
                    fpMulInstr3.StillHasDependencies() ? "wait" : "ready";
            }
            else
            {
                GetNodeByID(fpMul1).Annotations[0].Content = "";
                GetNodeByID(fpMulVal1).Annotations[0].Content = "";
            }

            var integerInstr1 = DPEx.integerReservationStation.instructions.ElementAtOrDefault(0);
            if (integerInstr1 != null)
            {
                GetNodeByID(integer3).Annotations[0].Content = integerInstr1.instruction.GetMnemonic();
                GetNodeByID(integerVal3).Annotations[0].Content =
                    integerInstr1.StillHasDependencies() ? "wait" : "ready";
            }
            else
            {
                GetNodeByID(integer3).Annotations[0].Content = "";
                GetNodeByID(integerVal3).Annotations[0].Content = "";
            }

            var integerInstr2 = DPEx.integerReservationStation.instructions.ElementAtOrDefault(1);
            if (integerInstr2 != null)
            {
                GetNodeByID(integer2).Annotations[0].Content = integerInstr2.instruction.GetMnemonic();
                GetNodeByID(integerVal2).Annotations[0].Content =
                    integerInstr2.StillHasDependencies() ? "wait" : "ready";
            }
            else
            {
                GetNodeByID(integer2).Annotations[0].Content = "";
                GetNodeByID(integerVal2).Annotations[0].Content = "";
            }

            var integerInstr3 = DPEx.integerReservationStation.instructions.ElementAtOrDefault(2);
            if (integerInstr3 != null)
            {
                GetNodeByID(integer1).Annotations[0].Content = integerInstr3.instruction.GetMnemonic();
                GetNodeByID(integerVal1).Annotations[0].Content =
                    integerInstr3.StillHasDependencies() ? "wait" : "ready";
            }
            else
            {
                GetNodeByID(integer1).Annotations[0].Content = "";
                GetNodeByID(integerVal1).Annotations[0].Content = "";
            }
        }

        void UpdateReorderBuffer()
        {
            var firstInstr = DPEx.reorderBuffer.buffers.UnorderedItems.ElementAtOrDefault(4).Element;
            if (firstInstr != null)
                GetNodeByID(reorderBuffer1).Annotations[0].Content = firstInstr.instruction.Disassemble();
            else
                GetNodeByID(reorderBuffer1).Annotations[0].Content = "";

            var secondInstr = DPEx.reorderBuffer.buffers.UnorderedItems.ElementAtOrDefault(3).Element;
            if (secondInstr != null)
                GetNodeByID(reorderBuffer2).Annotations[0].Content = secondInstr.instruction.Disassemble();
            else
                GetNodeByID(reorderBuffer2).Annotations[0].Content = "";

            var thirdInstr = DPEx.reorderBuffer.buffers.UnorderedItems.ElementAtOrDefault(2).Element;
            if (thirdInstr != null)
                GetNodeByID(reorderBuffer3).Annotations[0].Content = thirdInstr.instruction.Disassemble();
            else
                GetNodeByID(reorderBuffer3).Annotations[0].Content = "";

            var fourthInstr = DPEx.reorderBuffer.buffers.UnorderedItems.ElementAtOrDefault(1).Element;
            if (fourthInstr != null)
                GetNodeByID(reorderBuffer4).Annotations[0].Content = fourthInstr.instruction.Disassemble();
            else
                GetNodeByID(reorderBuffer4).Annotations[0].Content = "";

            var fifthInstr = DPEx.reorderBuffer.buffers.UnorderedItems.ElementAtOrDefault(0).Element;
            if (fifthInstr != null)
                GetNodeByID(reorderBuffer5).Annotations[0].Content = fifthInstr.instruction.Disassemble();
            else
                GetNodeByID(reorderBuffer5).Annotations[0].Content = "";
        }


        string fillColor => FrontendVariables.darkMode ? "darkgray" : "white";


        private void InitDynamicDiagramModel()
        {
            NodeCollection = new DiagramObjectCollection<Node>();
            ConnectorCollection = new DiagramObjectCollection<Connector>();

            #region Ports
            List<PointPort> ReorderBufferPorts1 = new List<PointPort>();
            ReorderBufferPorts1.Add(AddPort("port1ReorderBuffer", 0.2, 0));
            ReorderBufferPorts1.Add(AddPort("port2ReorderBuffer", 0.8, 0));
            ReorderBufferPorts1.Add(AddPort("port3ReorderBuffer", 0.5, 1));
            ReorderBufferPorts1.Add(AddPort("port4ReorderBuffer", 0.9, 1));

            List<PointPort> ReorderBufferPorts2 = new List<PointPort>();


            List<PointPort> InstrQueuePorts1 = new List<PointPort>();
            InstrQueuePorts1.Add(AddPort("port1InstrQueue", 0.5, 0));
            InstrQueuePorts1.Add(AddPort("port2InstrQueue", 0.4, 1));
            InstrQueuePorts1.Add(AddPort("port3InstrQueue", 0.7, 1));

            List<PointPort> InstrQueuePorts2 = new List<PointPort>();


            List<PointPort> RegFilePorts1 = new List<PointPort>();
            RegFilePorts1.Add(AddPort("port1RegFile", 0.41, 0));
            RegFilePorts1.Add(AddPort("port2RegFile", 0.695, 0));
            RegFilePorts1.Add(AddPort("port3RegFile", 0.1, 1));
            RegFilePorts1.Add(AddPort("port4RegFile", 0.2, 1));

            List<PointPort> RegFilePorts2 = new List<PointPort>();


            List<PointPort> IssueQueuePorts1 = new List<PointPort>();
            IssueQueuePorts1.Add(AddPort("port1IssueQueue", 0.57, 1));


            List<PointPort> AddressUnitPorts = new List<PointPort>();
            AddressUnitPorts.Add(AddPort("port1AddressUnit", 0.5, 0));
            AddressUnitPorts.Add(AddPort("port2AddressUnit", 0.6, 1));
            AddressUnitPorts.Add(AddPort("port3AddressUnit", 0, .5));
            AddressUnitPorts.Add(AddPort("port4AddressUnit", 1, .5));


            List<PointPort> LoadBufferPorts1 = new List<PointPort>();
            LoadBufferPorts1.Add(AddPort("port1LoadBuffer", 0.5, 0));
            LoadBufferPorts1.Add(AddPort("port2LoadBuffer", 0.15, 1));

            List<PointPort> LoadBufferPorts2 = new List<PointPort>();


            List<PointPort> MemoryUnitPorts = new List<PointPort>();
            MemoryUnitPorts.Add(AddPort("port1MemoryUnit", 0.1, 0));
            MemoryUnitPorts.Add(AddPort("port2MemoryUnit", 0.45, 0));
            MemoryUnitPorts.Add(AddPort("port3MemoryUnit", 0.89, 0));
            MemoryUnitPorts.Add(AddPort("port4MemoryUnit", 0.5, 1));


            List<PointPort> Res1Ports1 = new List<PointPort>();
            Res1Ports1.Add(AddPort("port1Res1", 0.5, 0));
            Res1Ports1.Add(AddPort("port2Res1", 0.5, 1));

            List<PointPort> Res1Ports2 = new List<PointPort>();


            List<PointPort> FPAdderPorts = new List<PointPort>();
            FPAdderPorts.Add(AddPort("port1FPAdder", 0.3, 0));
            FPAdderPorts.Add(AddPort("port2FPAdder", 0.95, 0));
            FPAdderPorts.Add(AddPort("port3FPAdder", 0.5, 1));


            List<PointPort> Res2Ports1 = new List<PointPort>();
            Res2Ports1.Add(AddPort("port1Res2", 0.5, 0));
            Res2Ports1.Add(AddPort("port2Res2", 0.5, 1));

            List<PointPort> Res2Ports2 = new List<PointPort>();


            List<PointPort> FPMultPorts = new List<PointPort>();
            FPMultPorts.Add(AddPort("port1FPMult", 0.25, 0));
            FPMultPorts.Add(AddPort("port2FPMult", 0.9, 0));
            FPMultPorts.Add(AddPort("port3FPMult", 0.5, 1));


            List<PointPort> Res3Ports1 = new List<PointPort>();
            Res3Ports1.Add(AddPort("port1Res3", 0.5, 0));
            Res3Ports1.Add(AddPort("port2Res3", 0.5, 1));

            List<PointPort> Res3Ports2 = new List<PointPort>();


            List<PointPort> IntUnitPorts = new List<PointPort>();
            IntUnitPorts.Add(AddPort("port1IntUnit", 0.3, 0));
            IntUnitPorts.Add(AddPort("port2IntUnit", 0.95, 0));
            IntUnitPorts.Add(AddPort("port3IntUnit", 0.5, 1));


            List<PointPort> CDBPorts = new List<PointPort>();
            CDBPorts.Add(AddPort("port1CDB", 0, 0.5));
            CDBPorts.Add(AddPort("port2CDB", 0.5, 0));
            CDBPorts.Add(AddPort("port3CDB", 1, 0.5));
            CDBPorts.Add(AddPort("port4CDB", 0.5, 1));

            List<PointPort> OpBusPorts = new List<PointPort>();
            OpBusPorts.Add(AddPort("port1OpBus", 0, 0.5));
            OpBusPorts.Add(AddPort("port2OpBus", 0.5, 0));
            OpBusPorts.Add(AddPort("port3OpBus", 1, 0.5));
            OpBusPorts.Add(AddPort("port4OpBus", 0.5, 1));

            List<PointPort> OperationBusPorts = new List<PointPort>();
            OperationBusPorts.Add(AddPort("port1OperationBus", 0, 0.5));
            OperationBusPorts.Add(AddPort("port2OperationBus", 0.5, 0));
            OperationBusPorts.Add(AddPort("port3OperationBus", 1, 0.5));
            OperationBusPorts.Add(AddPort("port4OperationBus", 0.5, 1));


            List<PointPort> WinSizePorts = new List<PointPort>();
            #endregion

            #region Variables To Move Node Groups and Entire Diagram
            // Move entire datapath
            int DatapathXOffset = 130;
            int DatapathYOffset = -20;

            // Change to move the entire RBuffer Component as a unit
            int RBufferXOffset = 600 + DatapathXOffset;
            int RBufferYOffset = 75 + DatapathYOffset;
            int RBufferLabelXOffset = 70;
            int RBufferItemWidth = 120;
            int RBufferItemHeight = 20;

            // Change to move the entire InstrQueue Component as a unit
            int InstrQueueXOffset = 350 + DatapathXOffset;
            int InstrQueueYOffset = 250 + DatapathYOffset;
            int InstrQueueLabelXOffset = 70;
            int InstrQueueItemWidth = 120;
            int InstrQueueItemHeight = 20;

            // Change to move the entire RegFile Component as a unit
            int RegFileXOffset = 615 + DatapathXOffset;
            int RegFileYOffset = 210 + DatapathYOffset;
            int RegFileItemWidth = 170;
            int RegFileItemHeight = 100;

            // Change to move the entire IssueQueue Component as a unit
            int IssueQueueXOffset = 340 + DatapathXOffset;
            int IssueQueueYOffset = 55 + DatapathYOffset;
            int IssueQueueItemWidth = 140;
            int IssueQueueItemHeight = 60;

            // Change to move the entire AddressUnit Component as a unit
            int AddressUnitXOffset = 130 + DatapathXOffset;
            int AddressUnitYOffset = 390 + DatapathYOffset;
            int AddressUnitItemWidth = 100;
            int AddressUnitItemHeight = 20;

            // Change to move the entire LoadBuffers Component as a unit
            int LoadBufferXOffset = 140 + DatapathXOffset;
            int LoadBufferYOffset = 440 + DatapathYOffset;
            int LoadBufferLabelXOffset = 40;
            int LoadBufferItemWidth = 60;
            int LoadBufferItemHeight = 20;

            // Change to move the entire MemoryUnit Component as a unit
            int MemoryUnitXOffset = 80 + DatapathXOffset;
            int MemoryUnitYOffset = 640 + DatapathYOffset;
            int MemoryUnitItemWidth = 100;
            int MemoryUnitItemHeight = 20;

            // Change to move the entire Res 1 Component as a unit
            int Res1XOffset = 220 + DatapathXOffset;
            int Res1YOffset = 545 + DatapathYOffset;
            int Res1LabelXOffset = 20;
            int Res1ItemWidth1 = 20;
            int Res1ItemWidth2 = 60;
            int Res1ItemWidth3 = 75;
            int Res1ItemHeight = 20;

            // Change to move the entire FP Adder Component as a unit
            int FPAdderXOffset = 275 + DatapathXOffset;
            int FPAdderYOffset = 640 + DatapathYOffset;
            int FPAdderItemWidth = 100;
            int FPAdderItemHeight = 20;

            // Change to move the entire Res 2 Component as a unit
            int Res2XOffset = 450 + DatapathXOffset;
            int Res2YOffset = 545 + DatapathYOffset;
            int Res2LabelXOffset = 20;
            int Res2ItemWidth1 = 20;
            int Res2ItemWidth2 = 60;
            int Res2ItemWidth3 = 75;
            int Res2ItemHeight = 20;

            // Change to move the entire FP Multiplier Component as a unit
            int FPMultXOffset = 510 + DatapathXOffset;
            int FPMultYOffset = 640 + DatapathYOffset;
            int FPMultItemWidth = 100;
            int FPMultItemHeight = 20;

            // Change to move the entire Res 3 Component as a unit
            int Res3XOffset = 670 + DatapathXOffset;
            int Res3YOffset = 545 + DatapathYOffset;
            int Res3LabelXOffset = 20;
            int Res3ItemWidth1 = 20;
            int Res3ItemWidth2 = 60;
            int Res3ItemWidth3 = 75;
            int Res3ItemHeight = 20;

            // CDB Node width and height
            int CDBNodeItemWidth = 3;
            int CDBNodeItemHeight = 3;

            // OpBus node Width and Height
            int OpBusNodeItemWidth = 3;
            int OpBusNodeItemHeight = 3;

            // OpBus node Width and Height
            int OperationBusNodeItemWidth = 3;
            int OperationBusNodeItemHeight = 3;

            // Change to move the entire Integer Unit Component as a unit
            int IntUnitXOffset = 725 + DatapathXOffset;
            int IntUnitYOffset = 640 + DatapathYOffset;
            int IntUnitItemWidth = 100;
            int IntUnitItemHeight = 20;

            // Change to move the entire CDB Component as a unit
            int CDBNode1XOffset = 80 + DatapathXOffset;
            int CDBNode1YOffset = 690 + DatapathYOffset;
            int CDBNode1ItemWidth = CDBNodeItemWidth;
            int CDBNode1ItemHeight = CDBNodeItemHeight;

            // Change to move the entire CDB Component as a unit
            int CDBNode2XOffset = 850 + DatapathXOffset;
            int CDBNode2YOffset = 690 + DatapathYOffset;
            int CDBNode2ItemWidth = CDBNodeItemWidth;
            int CDBNode2ItemHeight = CDBNodeItemHeight;

            // Change to move the entire CDB Component as a unit
            int CDBNode3XOffset = 850 + DatapathXOffset;
            int CDBNode3YOffset = 500 + DatapathYOffset;
            int CDBNode3ItemWidth = CDBNodeItemWidth;
            int CDBNode3ItemHeight = CDBNodeItemHeight;

            // Change to move the entire CDB FP Adder Component as a unit
            int CDBNode4XOffset = 275 + DatapathXOffset;
            int CDBNode4YOffset = 690 + DatapathYOffset;
            int CDBNode4ItemWidth = CDBNodeItemWidth;
            int CDBNode4ItemHeight = CDBNodeItemHeight;

            // Change to move the entire CDB FP Multiplier Component as a unit
            int CDBNode5XOffset = 510 + DatapathXOffset;
            int CDBNode5YOffset = 690 + DatapathYOffset;
            int CDBNode5ItemWidth = CDBNodeItemWidth;
            int CDBNode5ItemHeight = CDBNodeItemHeight;

            // Change to move the entire CDB Integer Unit Component as a unit
            int CDBNode6XOffset = 725 + DatapathXOffset;
            int CDBNode6YOffset = 690 + DatapathYOffset;
            int CDBNode6ItemWidth = CDBNodeItemWidth;
            int CDBNode6ItemHeight = CDBNodeItemHeight;

            // Change to move the entire CDB Component as a unit
            int CDBNode7XOffset = 255 + DatapathXOffset;
            int CDBNode7YOffset = 500 + DatapathYOffset;
            int CDBNode7ItemWidth = CDBNodeItemWidth;
            int CDBNode7ItemHeight = CDBNodeItemHeight;

            // Change to move the entire CDB Component as a unit
            int CDBNode8XOffset = 320 + DatapathXOffset;
            int CDBNode8YOffset = 500 + DatapathYOffset;
            int CDBNode8ItemWidth = CDBNodeItemWidth;
            int CDBNode8ItemHeight = CDBNodeItemHeight;

            // Change to move the entire CDB Component as a unit
            int CDBNode9XOffset = 485 + DatapathXOffset;
            int CDBNode9YOffset = 500 + DatapathYOffset;
            int CDBNode9ItemWidth = CDBNodeItemWidth;
            int CDBNode9ItemHeight = CDBNodeItemHeight;

            // Change to move the entire CDB Component as a unit
            int CDBNode10XOffset = 550 + DatapathXOffset;
            int CDBNode10YOffset = 500 + DatapathYOffset;
            int CDBNode10ItemWidth = CDBNodeItemWidth;
            int CDBNode10ItemHeight = CDBNodeItemHeight;

            // Change to move the entire CDB Component as a unit
            int CDBNode11XOffset = 705 + DatapathXOffset;
            int CDBNode11YOffset = 500 + DatapathYOffset;
            int CDBNode11ItemWidth = CDBNodeItemWidth;
            int CDBNode11ItemHeight = CDBNodeItemHeight;

            // Change to move the entire CDB Component as a unit
            int CDBNode12XOffset = 770 + DatapathXOffset;
            int CDBNode12YOffset = 500 + DatapathYOffset;
            int CDBNode12ItemWidth = CDBNodeItemWidth;
            int CDBNode12ItemHeight = CDBNodeItemHeight;

            // Change to move the entire CDB Component as a unit
            int CDBNode13XOffset = 850 + DatapathXOffset;
            int CDBNode13YOffset = 44 + DatapathYOffset;
            int CDBNode13ItemWidth = CDBNodeItemWidth;
            int CDBNode13ItemHeight = CDBNodeItemHeight;


            // Change to move the entire OpBus Component as a unit
            int OpBusNode1XOffset = 255 + DatapathXOffset;
            int OpBusNode1YOffset = 420 + DatapathYOffset;
            int OpBusNode1ItemWidth = OpBusNodeItemWidth;
            int OpBusNode1ItemHeight = OpBusNodeItemHeight;

            // Change to move the entire OpBus Component as a unit
            int OpBusNode2XOffset = 485 + DatapathXOffset;
            int OpBusNode2YOffset = 420 + DatapathYOffset;
            int OpBusNode2ItemWidth = OpBusNodeItemWidth;
            int OpBusNode2ItemHeight = OpBusNodeItemHeight;

            // Change to move the entire OpBus Component as a unit
            int OpBusNode3XOffset = 564 + DatapathXOffset;
            int OpBusNode3YOffset = 440 + DatapathYOffset;
            int OpBusNode3ItemWidth = OpBusNodeItemWidth;
            int OpBusNode3ItemHeight = OpBusNodeItemHeight;

            // Change to move the entire OpBus Component as a unit
            int OpBusNode4XOffset = 450 + DatapathXOffset;
            int OpBusNode4YOffset = 420 + DatapathYOffset;
            int OpBusNode4ItemWidth = OpBusNodeItemWidth;
            int OpBusNode4ItemHeight = OpBusNodeItemHeight;

            // Change to move the entire OpBus Component as a unit
            int OpBusNode5XOffset = 450 + DatapathXOffset;
            int OpBusNode5YOffset = 185 + DatapathYOffset;
            int OpBusNode5ItemWidth = OpBusNodeItemWidth;
            int OpBusNode5ItemHeight = OpBusNodeItemHeight;

            // Change to move the entire OpBus Component as a unit
            int OpBusNode6XOffset = 470 + DatapathXOffset;
            int OpBusNode6YOffset = 440 + DatapathYOffset;
            int OpBusNode6ItemWidth = OpBusNodeItemWidth;
            int OpBusNode6ItemHeight = OpBusNodeItemHeight;

            // Change to move the entire OpBus Component as a unit
            int OpBusNode7XOffset = 470 + DatapathXOffset;
            int OpBusNode7YOffset = 185 + DatapathYOffset;
            int OpBusNode7ItemWidth = OpBusNodeItemWidth;
            int OpBusNode7ItemHeight = OpBusNodeItemHeight;


            // Change to move the entire OperationBus Component as a unit
            int OperationBusNode1XOffset = 374 + DatapathXOffset;
            int OperationBusNode1YOffset = 460 + DatapathYOffset;
            int OperationBusNode1ItemWidth = OperationBusNodeItemWidth;
            int OperationBusNode1ItemHeight = OperationBusNodeItemHeight;

            

            #endregion

            #region Nodes
            CreateNode("ReorderBufferLabel", RBufferXOffset - (RBufferLabelXOffset), RBufferYOffset + 40, RBufferItemHeight * 5, RBufferItemHeight, -90, 0, ReorderBufferPorts2, FlowShapeType.Process, "Reorder Buffer", fillColor, "8,0", "black");
            CreateNode(reorderBuffer1, RBufferXOffset, RBufferYOffset, RBufferItemWidth, RBufferItemHeight, 0, 0, ReorderBufferPorts1, FlowShapeType.Process, "RBuffer 1", fillColor, "8,0", "black");
            CreateNode("ReorderBuffer2", RBufferXOffset, RBufferYOffset+20, RBufferItemWidth, RBufferItemHeight, 0, 0, ReorderBufferPorts2, FlowShapeType.Process, "RBuffer 2", fillColor, "8,0", "black");
            CreateNode("ReorderBuffer3", RBufferXOffset, RBufferYOffset+40, RBufferItemWidth, RBufferItemHeight, 0, 0, ReorderBufferPorts2, FlowShapeType.Process, "RBuffer 3", fillColor, "8,0", "black");
            CreateNode("ReorderBuffer4", RBufferXOffset, RBufferYOffset+60, RBufferItemWidth, RBufferItemHeight, 0, 0, ReorderBufferPorts2, FlowShapeType.Process, "RBuffer 4", fillColor, "8,0", "black");
            CreateNode("ReorderBuffer5", RBufferXOffset, RBufferYOffset+80, RBufferItemWidth, RBufferItemHeight, 0, 0, ReorderBufferPorts1, FlowShapeType.Process, "RBuffer 5", fillColor, "8,0", "black");

            CreateNode("InstrQueueLabel", InstrQueueXOffset - (InstrQueueLabelXOffset), InstrQueueYOffset + 40, InstrQueueItemHeight * 5, InstrQueueItemHeight, -90, 0, InstrQueuePorts2, FlowShapeType.Process, "Instr. Queue", fillColor, "8,0", "black");
            CreateNode(instrQueue1, InstrQueueXOffset, InstrQueueYOffset, InstrQueueItemWidth, InstrQueueItemHeight, 0, 0, InstrQueuePorts1, FlowShapeType.Process, "IQueue 1", fillColor, "8,0", "black");
            CreateNode("InstrQueue2", InstrQueueXOffset, InstrQueueYOffset + 20, InstrQueueItemWidth, InstrQueueItemHeight, 0, 0, InstrQueuePorts2, FlowShapeType.Process, "IQueue 2", fillColor, "8,0", "black");
            CreateNode("InstrQueue3", InstrQueueXOffset, InstrQueueYOffset + 40, InstrQueueItemWidth, InstrQueueItemHeight, 0, 0, InstrQueuePorts2, FlowShapeType.Process, "IQueue 3", fillColor, "8,0", "black");
            CreateNode("InstrQueue4", InstrQueueXOffset, InstrQueueYOffset + 60, InstrQueueItemWidth, InstrQueueItemHeight, 0, 0, InstrQueuePorts2, FlowShapeType.Process, "IQueue 4", fillColor, "8,0", "black");
            CreateNode("InstrQueue5", InstrQueueXOffset, InstrQueueYOffset + 80, InstrQueueItemWidth, InstrQueueItemHeight, 0, 0, InstrQueuePorts1, FlowShapeType.Process, "IQueue 5", fillColor, "8,0", "black");

            CreateNode("RegisterFile", RegFileXOffset, RegFileYOffset + 80, RegFileItemWidth, RegFileItemHeight, 0, 0, RegFilePorts1, FlowShapeType.Process, "I/F Register File", "darkgray", "8,0", "black");

            CreateNode("IssueUnit", IssueQueueXOffset, IssueQueueYOffset + 80, IssueQueueItemWidth, IssueQueueItemHeight, 0, 0, IssueQueuePorts1, FlowShapeType.Process, "Issue Unit", "darkgray", "8,0", "black");

            CreateNode("AddressUnit", AddressUnitXOffset, AddressUnitYOffset, AddressUnitItemWidth, AddressUnitItemHeight, 0, 0, AddressUnitPorts, FlowShapeType.Process, "Address Unit", "darkgray", "8,0", "black");

            CreateNode("LoadBufferLabel", LoadBufferXOffset - (LoadBufferLabelXOffset), LoadBufferYOffset + 40, LoadBufferItemHeight * 5, LoadBufferItemHeight, -90, 0, LoadBufferPorts2, FlowShapeType.Process, "Load Buffers", fillColor, "8,0", "black");
            CreateNode("LoadBuffer1", LoadBufferXOffset, LoadBufferYOffset, LoadBufferItemWidth, LoadBufferItemHeight, 0, 0, LoadBufferPorts1, FlowShapeType.Process, "LoadBuff 1", fillColor, "8,0", "black");
            CreateNode("LoadBuffer2", LoadBufferXOffset, LoadBufferYOffset + 20, LoadBufferItemWidth, LoadBufferItemHeight, 0, 0, LoadBufferPorts2, FlowShapeType.Process, "LoadBuff 2", fillColor, "8,0", "black");
            CreateNode("LoadBuffer3", LoadBufferXOffset, LoadBufferYOffset + 40, LoadBufferItemWidth, LoadBufferItemHeight, 0, 0, LoadBufferPorts2, FlowShapeType.Process, "LoadBuff 3", fillColor, "8,0", "black");
            CreateNode("LoadBuffer4", LoadBufferXOffset, LoadBufferYOffset + 60, LoadBufferItemWidth, LoadBufferItemHeight, 0, 0, LoadBufferPorts2, FlowShapeType.Process, "LoadBuff 4", fillColor, "8,0", "black");
            CreateNode("LoadBuffer5", LoadBufferXOffset, LoadBufferYOffset + 80, LoadBufferItemWidth, LoadBufferItemHeight, 0, 0, LoadBufferPorts1, FlowShapeType.Process, "LoadBuff 5", fillColor, "8,0", "black");

            CreateNode("MemoryUnit", MemoryUnitXOffset, MemoryUnitYOffset, MemoryUnitItemWidth, MemoryUnitItemHeight, 0, 0, MemoryUnitPorts, FlowShapeType.Process, "Memory Unit", "darkgray", "8,0", "black");

            CreateNode("ResStation1Label", Res1XOffset - (Res1LabelXOffset), Res1YOffset + 20, Res1ItemHeight * 3, Res1ItemHeight, -90, 0, Res1Ports2, FlowShapeType.Process, "Res 1", fillColor, "8,0", "black");
            CreateNode("Res1OpBus1", Res1XOffset, Res1YOffset, Res1ItemWidth1, Res1ItemHeight, 0, 0, Res1Ports1, FlowShapeType.Process, "3", fillColor, "8,0", "black");
            CreateNode("Res1OpBus2", Res1XOffset, Res1YOffset + 20, Res1ItemWidth1, Res1ItemHeight, 0, 0, Res1Ports2, FlowShapeType.Process, "2", fillColor, "8,0", "black");
            CreateNode("Res1OpBus3", Res1XOffset, Res1YOffset + 40, Res1ItemWidth1, Res1ItemHeight, 0, 0, Res1Ports2, FlowShapeType.Process, "1", fillColor, "8,0", "black");
            CreateNode("Res1DataBus1", Res1XOffset + (Res1ItemWidth1 + 15), Res1YOffset, Res1ItemWidth2, Res1ItemHeight, 0, 0, Res1Ports1, FlowShapeType.Process, "", fillColor, "8,0", "black");
            CreateNode("Res1DataBus2", Res1XOffset + (Res1ItemWidth1 + 15), Res1YOffset + 20, Res1ItemWidth2, Res1ItemHeight, 0, 0, Res1Ports2, FlowShapeType.Process, "", fillColor, "8,0", "black");
            CreateNode("Res1DataBus3", Res1XOffset + (Res1ItemWidth1 + 15), Res1YOffset + 40, Res1ItemWidth2, Res1ItemHeight, 0, 0, Res1Ports1, FlowShapeType.Process, "", fillColor, "8,0", "black");
            CreateNode("Res1OperandBus1", Res1XOffset + (Res1ItemWidth2 + Res1ItemWidth1 + 20), Res1YOffset, Res1ItemWidth3, Res1ItemHeight, 0, 0, Res1Ports1, FlowShapeType.Process, "", fillColor, "8,0", "black");
            CreateNode("Res1OperandBus2", Res1XOffset + (Res1ItemWidth2 + Res1ItemWidth1 + 20), Res1YOffset + 20, Res1ItemWidth3, Res1ItemHeight, 0, 0, Res1Ports2, FlowShapeType.Process, "", fillColor, "8,0", "black");
            CreateNode("Res1OperandBus3", Res1XOffset + (Res1ItemWidth2 + Res1ItemWidth1 + 20), Res1YOffset + 40, Res1ItemWidth3, Res1ItemHeight, 0, 0, Res1Ports1, FlowShapeType.Process, "", fillColor, "8,0", "black");

            CreateNode("FPAdder", FPAdderXOffset, FPAdderYOffset, FPAdderItemWidth, FPAdderItemHeight, 0, 0, FPAdderPorts, FlowShapeType.Process, "FP Adder", "darkgray", "8,0", "black");

            CreateNode("ResStation2Label", Res2XOffset - (Res2LabelXOffset), Res2YOffset + 20, Res2ItemHeight * 3, Res2ItemHeight, -90, 0, Res2Ports2, FlowShapeType.Process, "Res 2", fillColor, "8,0", "black");
            CreateNode("Res2OpBus1", Res2XOffset, Res2YOffset, Res2ItemWidth1, Res2ItemHeight, 0, 0, Res2Ports1, FlowShapeType.Process, "3", fillColor, "8,0", "black");
            CreateNode("Res2OpBus2", Res2XOffset, Res2YOffset + 20, Res2ItemWidth1, Res2ItemHeight, 0, 0, Res2Ports2, FlowShapeType.Process, "2", fillColor, "8,0", "black");
            CreateNode("Res2OpBus3", Res2XOffset, Res2YOffset + 40, Res2ItemWidth1, Res2ItemHeight, 0, 0, Res2Ports2, FlowShapeType.Process, "1", fillColor, "8,0", "black");
            CreateNode("Res2DataBus1", Res2XOffset + (Res2ItemWidth1 + 15), Res2YOffset, Res2ItemWidth2, Res2ItemHeight, 0, 0, Res2Ports1, FlowShapeType.Process, "", fillColor, "8,0", "black");
            CreateNode("Res2DataBus2", Res2XOffset + (Res2ItemWidth1 + 15), Res2YOffset + 20, Res2ItemWidth2, Res2ItemHeight, 0, 0, Res2Ports2, FlowShapeType.Process, "", fillColor, "8,0", "black");
            CreateNode("Res2DataBus3", Res2XOffset + (Res2ItemWidth1 + 15), Res2YOffset + 40, Res2ItemWidth2, Res2ItemHeight, 0, 0, Res2Ports1, FlowShapeType.Process, "", fillColor, "8,0", "black");
            CreateNode("Res2OperandBus1", Res2XOffset + (Res2ItemWidth2 + Res2ItemWidth1 + 20), Res2YOffset, Res2ItemWidth3, Res2ItemHeight, 0, 0, Res2Ports1, FlowShapeType.Process, "", fillColor, "8,0", "black");
            CreateNode("Res2OperandBus2", Res2XOffset + (Res2ItemWidth2 + Res2ItemWidth1 + 20), Res2YOffset + 20, Res2ItemWidth3, Res2ItemHeight, 0, 0, Res2Ports2, FlowShapeType.Process, "", fillColor, "8,0", "black");
            CreateNode("Res2OperandBus3", Res2XOffset + (Res2ItemWidth2 + Res2ItemWidth1 + 20), Res2YOffset + 40, Res2ItemWidth3, Res2ItemHeight, 0, 0, Res2Ports1, FlowShapeType.Process, "", fillColor, "8,0", "black");

            CreateNode("FPMult", FPMultXOffset, FPMultYOffset, FPMultItemWidth, FPMultItemHeight, 0, 0, FPMultPorts, FlowShapeType.Process, "FP Multiplier", "darkgray", "8,0", "black");

            CreateNode("ResStation3Label", Res3XOffset - (Res3LabelXOffset), Res3YOffset + 20, Res3ItemHeight * 3, Res3ItemHeight, -90, 0, Res3Ports2, FlowShapeType.Process, "Res 3", fillColor, "8,0", "black");
            CreateNode("Res3OpBus1", Res3XOffset, Res3YOffset, Res3ItemWidth1, Res3ItemHeight, 0, 0, Res3Ports1, FlowShapeType.Process, "3", fillColor, "8,0", "black");
            CreateNode("Res3OpBus2", Res3XOffset, Res3YOffset + 20, Res3ItemWidth1, Res3ItemHeight, 0, 0, Res3Ports2, FlowShapeType.Process, "2", fillColor, "8,0", "black");
            CreateNode("Res3OpBus3", Res3XOffset, Res3YOffset + 40, Res3ItemWidth1, Res3ItemHeight, 0, 0, Res3Ports2, FlowShapeType.Process, "1", fillColor, "8,0", "black");
            CreateNode("Res3DataBus1", Res3XOffset + (Res3ItemWidth1 + 15), Res3YOffset, Res3ItemWidth2, Res3ItemHeight, 0, 0, Res3Ports1, FlowShapeType.Process, "", fillColor, "8,0", "black");
            CreateNode("Res3DataBus2", Res3XOffset + (Res3ItemWidth1 + 15), Res3YOffset + 20, Res3ItemWidth2, Res3ItemHeight, 0, 0, Res3Ports2, FlowShapeType.Process, "", fillColor, "8,0", "black");
            CreateNode("Res3DataBus3", Res3XOffset + (Res3ItemWidth1 + 15), Res3YOffset + 40, Res3ItemWidth2, Res3ItemHeight, 0, 0, Res3Ports1, FlowShapeType.Process, "", fillColor, "8,0", "black");
            CreateNode("Res3OperandBus1", Res3XOffset + (Res3ItemWidth2 + Res3ItemWidth1 + 20), Res3YOffset, Res3ItemWidth3, Res3ItemHeight, 0, 0, Res3Ports1, FlowShapeType.Process, "", fillColor, "8,0", "black");
            CreateNode("Res3OperandBus2", Res3XOffset + (Res3ItemWidth2 + Res3ItemWidth1 + 20), Res3YOffset + 20, Res3ItemWidth3, Res3ItemHeight, 0, 0, Res3Ports2, FlowShapeType.Process, "", fillColor, "8,0", "black");
            CreateNode("Res3OperandBus3", Res3XOffset + (Res3ItemWidth2 + Res3ItemWidth1 + 20), Res3YOffset + 40, Res3ItemWidth3, Res3ItemHeight, 0, 0, Res3Ports1, FlowShapeType.Process, "", fillColor, "8,0", "black");

            CreateNode("IntUnit", IntUnitXOffset, IntUnitYOffset, IntUnitItemWidth, IntUnitItemHeight, 0, 0, IntUnitPorts, FlowShapeType.Process, "Integer Unit", "darkgray", "8,0", "black");



            // CDB Nodes
            CreateNode("CDBNode1", CDBNode1XOffset, CDBNode1YOffset, CDBNode1ItemWidth, CDBNode1ItemHeight, 0, 0, CDBPorts, FlowShapeType.Process, "", "black", "8,0", "black");
            CreateNode("CDBNode2", CDBNode2XOffset, CDBNode2YOffset, CDBNode2ItemWidth, CDBNode2ItemHeight, 0, 0, CDBPorts, FlowShapeType.Process, "", "black", "8,0", "black");
            CreateNode("CDBNode3", CDBNode3XOffset, CDBNode3YOffset, CDBNode3ItemWidth, CDBNode3ItemHeight, 0, 0, CDBPorts, FlowShapeType.Process, "", "black", "8,0", "black");

            CreateNode("CDBFPAdderNode", CDBNode4XOffset, CDBNode4YOffset, CDBNode4ItemWidth, CDBNode4ItemHeight, 0, 0, CDBPorts, FlowShapeType.Process, "", "black", "8,0", "black");
            CreateNode("CDBFPMultNode", CDBNode5XOffset, CDBNode5YOffset, CDBNode5ItemWidth, CDBNode5ItemHeight, 0, 0, CDBPorts, FlowShapeType.Process, "", "black", "8,0", "black");
            CreateNode("CDBIntegerUnitNode", CDBNode6XOffset, CDBNode6YOffset, CDBNode6ItemWidth, CDBNode6ItemHeight, 0, 0, CDBPorts, FlowShapeType.Process, "", "black", "8,0", "black");

            CreateNode("CDBNode7", CDBNode7XOffset, CDBNode7YOffset, CDBNode7ItemWidth, CDBNode7ItemHeight, 0, 0, CDBPorts, FlowShapeType.Process, "", "black", "8,0", "black");
            CreateNode("CDBNode8", CDBNode8XOffset, CDBNode8YOffset, CDBNode8ItemWidth, CDBNode8ItemHeight, 0, 0, CDBPorts, FlowShapeType.Process, "", "black", "8,0", "black");
            CreateNode("CDBNode9", CDBNode9XOffset, CDBNode9YOffset, CDBNode9ItemWidth, CDBNode9ItemHeight, 0, 0, CDBPorts, FlowShapeType.Process, "", "black", "8,0", "black");
            CreateNode("CDBNode10", CDBNode10XOffset, CDBNode10YOffset, CDBNode10ItemWidth, CDBNode10ItemHeight, 0, 0, CDBPorts, FlowShapeType.Process, "", "black", "8,0", "black");
            CreateNode("CDBNode11", CDBNode11XOffset, CDBNode11YOffset, CDBNode11ItemWidth, CDBNode11ItemHeight, 0, 0, CDBPorts, FlowShapeType.Process, "", "black", "8,0", "black");
            CreateNode("CDBNode12", CDBNode12XOffset, CDBNode12YOffset, CDBNode12ItemWidth, CDBNode12ItemHeight, 0, 0, CDBPorts, FlowShapeType.Process, "", "black", "8,0", "black");

            CreateNode("CDBNode13", CDBNode13XOffset, CDBNode13YOffset, CDBNode13ItemWidth, CDBNode13ItemHeight, 0, 0, CDBPorts, FlowShapeType.Process, "", "black", "8,0", "black");


            // Operand Bus Nodes
            CreateNode("OpBusNode1", OpBusNode1XOffset, OpBusNode1YOffset, OpBusNode1ItemWidth, OpBusNode1ItemHeight, 0, 0, OpBusPorts, FlowShapeType.Process, "", "black", "8,0", "black");
            CreateNode("OpBusNode2", OpBusNode2XOffset, OpBusNode2YOffset, OpBusNode2ItemWidth, OpBusNode2ItemHeight, 0, 0, OpBusPorts, FlowShapeType.Process, "", "black", "8,0", "black");
            CreateNode("OpBusNode3", OpBusNode3XOffset, OpBusNode3YOffset, OpBusNode3ItemWidth, OpBusNode3ItemHeight, 0, 0, OpBusPorts, FlowShapeType.Process, "", "black", "8,0", "black");
            CreateNode("OpBusNode4", OpBusNode4XOffset, OpBusNode4YOffset, OpBusNode4ItemWidth, OpBusNode4ItemHeight, 0, 0, OpBusPorts, FlowShapeType.Process, "", "black", "8,0", "black");
            CreateNode("OpBusNode5", OpBusNode5XOffset, OpBusNode5YOffset, OpBusNode5ItemWidth, OpBusNode5ItemHeight, 0, 0, OpBusPorts, FlowShapeType.Process, "", "black", "8,0", "black");
            CreateNode("OpBusNode6", OpBusNode6XOffset, OpBusNode6YOffset, OpBusNode6ItemWidth, OpBusNode6ItemHeight, 0, 0, OpBusPorts, FlowShapeType.Process, "", "black", "8,0", "black");
            CreateNode("OpBusNode7", OpBusNode7XOffset, OpBusNode7YOffset, OpBusNode7ItemWidth, OpBusNode7ItemHeight, 0, 0, OpBusPorts, FlowShapeType.Process, "", "black", "8,0", "black");


            // Operation Bus Nodes
            CreateNode("OperationBusNode1", OperationBusNode1XOffset, OperationBusNode1YOffset, OperationBusNode1ItemWidth, OperationBusNode1ItemHeight, 0, 0, OperationBusPorts, FlowShapeType.Process, "", "black", "8,0", "black");
            

            CreateNode("sizeNodeYX", 1170, 700, 1, 1, 0, 0, WinSizePorts, FlowShapeType.Process, "", fillColor, "8,0", fillColor);
            #endregion

            #region Connectors
            CreateConnector(ReorderBuffToRegFileData, "ReorderBuffer5", "port4ReorderBuffer", "RegisterFile", "port2RegFile", "8,0", "black");
            CreateConnector(ReorderBuffToRegFileRegNum, "ReorderBuffer5", "port3ReorderBuffer", "RegisterFile", "port1RegFile", "8,0", "black");

            CreateConnector(IssueQueueToInstrQueue, "IssueUnit", "port1IssueQueue", "InstrQueue1", "port1InstrQueue", "8,0", "black");

            CreateConnector(InstrQueueToAddressUnit, "InstrQueue5", "port2InstrQueue", "AddressUnit", "port1AddressUnit", "8,0", "black", "Load/Store\nOperations", AnnotationAlignment.After, 0.8);

            CreateConnector(AddressUnitToLoadBuffers, "AddressUnit", "port2AddressUnit", "LoadBuffer1", "port1LoadBuffer", "8,0", "black");

            CreateConnector(LoadBufferToMemoryUnit, "LoadBuffer5", "port2LoadBuffer", "MemoryUnit", "port3MemoryUnit", "8,0", "black", "Load Addr", AnnotationAlignment.Center, .8);

            CreateConnector(ReorderBufferDataToMemoryUnit, "ReorderBuffer5", "port4ReorderBuffer", "MemoryUnit", "port1MemoryUnit", "8,0", "black", "Store Data", AnnotationAlignment.Center, .98);

            CreateConnector(ReorderBufferStoreAddrToMemoryUnit, "ReorderBuffer5", "port3ReorderBuffer", "MemoryUnit", "port2MemoryUnit", "8,0", "black", "Store Addr", AnnotationAlignment.Center, .95);

            CreateConnector(AddressUnitToReorderBuffer, "AddressUnit", "port3AddressUnit", "ReorderBuffer1", "port1ReorderBuffer", "8,0", "black");

            CreateConnector(Res1DataBusToFPAdder1, "Res1DataBus3", "port2Res1", "FPAdder", "port1FPAdder", "8,0", "black");
            CreateConnector(Res1OperandBusToFPAdder1, "Res1OperandBus3", "port2Res1", "FPAdder", "port2FPAdder", "8,0", "black");

            CreateConnector(Res2DataBusToFPMult1, "Res2DataBus3", "port2Res2", "FPMult", "port1FPMult", "8,0", "black");
            CreateConnector(Res2OperandBusToFPMult1, "Res2OperandBus3", "port2Res2", "FPMult", "port2FPMult", "8,0", "black");

            CreateConnector(Res3DataBusToIntUnit1, "Res3DataBus3", "port2Res3", "IntUnit", "port1IntUnit", "8,0", "black");
            CreateConnector(Res3OperandBusToIntUnit1, "Res3OperandBus3", "port2Res3", "IntUnit", "port2IntUnit", "8,0", "black");




            // CDB Connectors
            CreateConnector(CDBNode1ToCDBNode2, "CDBNode1", "port3CDB", "CDBNode2", "port1CDB", "8,0", "black", "Common Data Bus", AnnotationAlignment.After, 0.5, default, default, default, DecoratorShape.None);
            CreateConnector(MemoryUnitToCDBNode1, "MemoryUnit", "port4MemoryUnit", "CDBNode1", "port2CDB", "8,0", "black");
            CreateConnector(FPAdderToCDBNode4, "FPAdder", "port3FPAdder", "CDBFPAdderNode", "port2CDB", "8,0", "black");
            CreateConnector(FPMultToCDBNode5, "FPMult", "port3FPMult", "CDBFPMultNode", "port2CDB", "8,0", "black");
            CreateConnector(IntUnitToCDBNode6, "IntUnit", "port3IntUnit", "CDBIntegerUnitNode", "port2CDB", "8,0", "black");
            CreateConnector(CDBNode3ToCDBNode7, "CDBNode3", "port1CDB", "CDBNode7", "port3CDB", "8,0", "black", default, default, default, default, default, default, DecoratorShape.None);
            CreateConnector(CDBNode2ToCDBNode13, "CDBNode2", "port2CDB", "CDBNode13", "port4CDB", "8,0", "black", default, default, default, default, default, default, DecoratorShape.None);
            CreateConnector(CDBNode13ToReorderBuffer, "CDBNode13", "port1CDB", "ReorderBuffer1", "port2ReorderBuffer", "8,0", "black");


            // OPBus Connectors
            CreateConnector(RegFile5ToOpBusNode2, "RegisterFile", "port3RegFile", "OpBusNode2", "port3OpBus", "8,0", "black", default, default, default, default, default, default, DecoratorShape.None);
            CreateConnector(OpBusNode2ToRes1DataBus, "OpBusNode2", "port1OpBus", "Res1DataBus1", "port1Res1", "8,0", "black", "FP Operations", AnnotationAlignment.After, 0.35);
            CreateConnector(OpBusNode1ToAddressUnit, "OpBusNode1", "port2OpBus", "AddressUnit", "port4AddressUnit", "8,0", "black");
            CreateConnector(DataToOpBusNode4, "OpBusNode5", "port4OpBus", "OpBusNode4", "port2OpBus", "8,0", "black");
            CreateConnector(DataToOpBusNode6, "OpBusNode7", "port4OpBus", "OpBusNode6", "port2OpBus", "8,0", "black");
            CreateConnector(RegFileToOpBusNode3, "RegisterFile", "port4RegFile", "OpBusNode3", "port2OpBus", "8,0", "black", "Operand\nBuses", AnnotationAlignment.After, 0.2);
            CreateConnector(OpBusNode2ToRes2DataBus, "OpBusNode2", "port4OpBus", "Res2DataBus1", "port1Res2", "8,0", "black");
            CreateConnector(OpBusNode2ToRes3DataBus, "OpBusNode2", "port3OpBus", "Res3DataBus1", "port1Res3", "8,0", "black");
            CreateConnector(OpBusNode3ToRes1OperandBus, "OpBusNode3", "port1OpBus", "Res1OperandBus1", "port1Res1", "8,0", "black");
            CreateConnector(OpBusNode3ToRes2OperandBus, "OpBusNode3", "port1OpBus", "Res2OperandBus1", "port1Res2", "8,0", "black");
            CreateConnector(OpBusNode3ToRes3OperandBus, "OpBusNode3", "port3OpBus", "Res3OperandBus1", "port1Res3", "8,0", "black");


            // OperationBus Connectors
            CreateConnector(InstrQueueToOperationBus, "InstrQueue5", "port3InstrQueue", "OperationBusNode1", "port2OperationBus", "8,0", "black");
            CreateConnector(OperationBusToRes1OpBus, "OperationBusNode1", "port1OperationBus", "Res1OpBus1", "port1OpBus", "8,0", "black");
            CreateConnector(OperationBusToRes2OpBus, "OperationBusNode1", "port3OperationBus", "Res2OpBus1", "port1OpBus", "8,0", "black", "Operation Bus", AnnotationAlignment.After, 0);
            CreateConnector(OperationBusToRes3OpBus, "OperationBusNode1", "port3OperationBus", "Res3OpBus1", "port1OpBus", "8,0", "black");


            #endregion

            UpdateDynamicDiagram();
        }

        #region Dynamic Connector Strings
        // Connector Strings
        private string ReorderBuffToRegFileData = "ReorderBuffToRegFileData";
        private string ReorderBuffToRegFileRegNum = "ReorderBuffToRegFileRegNum";
        private string IssueQueueToInstrQueue = "IssueQueueToInstrQueue";
        private string InstrQueueToAddressUnit = "InstrQueueToAddressUnit";
        private string AddressUnitToLoadBuffers = "AddressUnitToLoadBuffers";
        private string LoadBufferToMemoryUnit = "LoadBufferToMemoryUnit";
        private string ReorderBufferDataToMemoryUnit = "ReorderBufferDataToMemoryUnit";
        private string ReorderBufferStoreAddrToMemoryUnit = "ReorderBufferStoreAddrToMemoryUnit";
        private string AddressUnitToReorderBuffer = "AddressUnitToReorderBuffer";

        private string Res1DataBusToFPAdder1 = "Res1DataBusToFPAdder1";
        private string Res1OperandBusToFPAdder1 = "Res1OperandBusToFPAdder1";

        private string Res2DataBusToFPMult1 = "Res2DataBusToFPMult1";
        private string Res2OperandBusToFPMult1 = "Res2OperandBusToFPMult1";

        private string Res3DataBusToIntUnit1 = "Res3DataBusToIntUnit1";
        private string Res3OperandBusToIntUnit1 = "Res3OperandBusToIntUnit1";

        // CDB strings
        private string CDBNode1ToCDBNode2 = "CDBNode1ToCDBNode2";
        private string MemoryUnitToCDBNode1 = "MemoryUnitToCDBNode1";
        private string FPAdderToCDBNode4 = "FPAdderToCDBNode4";
        private string FPMultToCDBNode5 = "FPMultToCDBNode5";
        private string IntUnitToCDBNode6 = "IntUnitToCDBNode6";
        private string CDBNode3ToCDBNode7 = "CDBNode3ToCDBNode7";
        private string CDBNode2ToCDBNode13 = "CDBNode2ToCDBNode13";
        private string CDBNode13ToReorderBuffer = "CDBNode13ToReorderBuffer";

        // OpBus strings
        private string RegFile5ToOpBusNode2 = "RegFile5ToOpBusNode2";
        private string OpBusNode2ToRes1DataBus = "OpBusNode2ToRes1DataBus";
        private string OpBusNode1ToAddressUnit = "OpBusNode1ToAddressUnit";
        private string DataToOpBusNode4 = "DataToOpBusNode4";
        private string DataToOpBusNode6 = "DataToOpBusNode6";
        private string RegFileToOpBusNode3 = "RegFileToOpBusNode3";
        private string OpBusNode2ToRes2DataBus = "OpBusNode2ToRes2DataBus";
        private string OpBusNode2ToRes3DataBus = "OpBusNode2ToRes3DataBus";
        private string OpBusNode3ToRes1OperandBus = "OpBusNode3ToRes1OperandBus";
        private string OpBusNode3ToRes2OperandBus = "OpBusNode3ToRes2OperandBus";
        private string OpBusNode3ToRes3OperandBus = "OpBusNode3ToRes3OperandBus";

        // Operation Bus Strings
        private string InstrQueueToOperationBus = "InstrQueueToOperationBus";
        private string OperationBusToRes1OpBus = "OperationBusToRes1OpBus";
        private string OperationBusToRes2OpBus = "OperationBusToRes2OpBus";
        private string OperationBusToRes3OpBus = "OperationBusToRes3OpBus";

        #endregion

        #region Static Diagram Updates

        void UpdateStaticDiagram()
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

        Node GetNodeByID(string id)
        {
            var node = NodeCollection.FirstOrDefault(node => node.ID == id);
            if (node == null)
                throw new Exception($"Node not found with ID: {id}");
            return node;
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

        #endregion

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

        private void InitStaticDiagramModel()
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
            CreateNode("FetchMux", 60, 293, 60, 27, -90, 90, FetchMuxPorts, FlowShapeType.Terminator, "Mux", fillColor, "8,0", "black");
            CreateNode("PC", 110, 293, 25, 55, 0, 0, PCPorts, FlowShapeType.Process, "PC", fillColor, "8,0", "black");
            CreateNode("InstrMem", 200, 370, 100, 100, 0, 0, InstrMemPorts, FlowShapeType.Process, "Instruction Memory", fillColor, "8,0", "black");
            CreateNode("AddPC", 205, 250, 75, 70, -90, 90, AddPCPorts, BasicShapeType.Trapezoid, "Add", fillColor, "8,0", "black");

            CreateNode("IFID", 300, 343, 30, 450, 0, -90, ifidPorts, FlowShapeType.Process, "IF/ID", fillColor, "8,0", "black", default, new DiagramPoint(.5, .1));

            // Decode Nodes
            rdReg = new ShapeAnnotation()
            {
                Content = "null",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Offset = new DiagramPoint(0.1, .71),
                Style = new TextStyle() {Color = "red", FontSize = 9.0 }
            };
            CreateNode("Registers", 450, 350, 100, 100, 0, 0, regPorts, FlowShapeType.Process, "Registers", fillColor, "8,0", "black", default, null, rdReg);
            CreateNode("ImmGen", 470, 450, 40, 75, 0, 0, ImmGenPorts, BasicShapeType.Ellipse, "Imm Gen", fillColor, "8,0", "black");
            CreateNode("Control", 480, 70, 45, 100, 0, 0, ControlPorts, BasicShapeType.Ellipse, "Control", fillColor, ControlDash, "black");

            CreateNode("IDEX", 560, 343, 30, 450, 0, -90, idexPorts, FlowShapeType.Process, "ID/EX", fillColor, "8,0", "black", default, new DiagramPoint(.5, .1));
            CreateNode("RW", 560, 33, 35, 15, 0, 0, RWPorts, FlowShapeType.Process, "RW", fillColor, ControlDash, "black");
            CreateNode("MTR", 560, 48, 35, 15, 0, 0, MTRPorts, FlowShapeType.Process, "MTR", fillColor, ControlDash, "black");
            CreateNode("MR", 560, 63, 35, 15, 0, 0, MRPorts, FlowShapeType.Process, "MR", fillColor, ControlDash, "black");
            CreateNode("MW", 560, 78, 35, 15, 0, 0, MWPorts, FlowShapeType.Process, "MW", fillColor, ControlDash, "black");
            CreateNode("PCS", 560, 93, 35, 15, 0, 0, PCSPorts, FlowShapeType.Process, "PCS", fillColor, ControlDash, "black");
            CreateNode("ALUS", 560, 108, 35, 15, 0, 0, ALUSPorts, FlowShapeType.Process, "ALUS", fillColor, ControlDash, "black");

            // Execute Nodes
            CreateNode("ExecuteMux", 632, 378, 60, 27, -90, 90, ExecuteMuxPorts, FlowShapeType.Terminator, "Mux", fillColor, "8,0", "black");
            CreateNode("ALU", 710, 281, 75, 70, -90, 90, ALUPorts, BasicShapeType.Trapezoid, "ALU", fillColor, "8,0", "black");
            //CreateNode("ALUControl", 640, 490, 45, 75, 0, 0, ImmGenPorts, BasicShapeType.Ellipse, "ALU Control", fillColor, ControlOpacity, "black");

            CreateNode("EXMEM", 800, 343, 30, 450, 0, -90, exmemPorts, FlowShapeType.Process, "EX/MEM", fillColor, "8,0", "black", default, new DiagramPoint(.5, .1));
            CreateNode("RW1", 800, 48, 35, 15, 0, 0, RW1Ports, FlowShapeType.Process, "RW", fillColor, ControlDash, "black");
            CreateNode("MTR1", 800, 63, 35, 15, 0, 0, MTR1Ports, FlowShapeType.Process, "MTR", fillColor, ControlDash, "black");
            CreateNode("MR1", 800, 78, 35, 15, 0, 0, MR1Ports, FlowShapeType.Process, "MR", fillColor, ControlDash, "black");
            CreateNode("MW1", 800, 93, 35, 15, 0, 0, MW1Ports, FlowShapeType.Process, "MW", fillColor, ControlDash, "black");
            CreateNode("PCS1", 800, 108, 35, 15, 0, 0, PCS1Ports, FlowShapeType.Process, "PCS", fillColor, ControlDash, "black");

            // Memory Nodes
            CreateNode("DataMem", 950, 319, 100, 100, 0, 0, dataMemPorts, FlowShapeType.Process, "Data Memory", fillColor, "8,0", "black");
            CreateNode("CheckFlags", 880, 164, 75, 65, -90, 90, ChkFlgPorts, BasicShapeType.Trapezoid, "Check Flgs", fillColor, ControlDash, "black");

            CreateNode("MEMWB", 1055, 343, 30, 450, 0, -90, memwbPorts, FlowShapeType.Process, "MEM/WB", fillColor, "8,0", "black", default, new DiagramPoint(.5, .1));
            CreateNode("RW2", 1055, 93, 35, 15, 0, 0, RW2Ports, FlowShapeType.Process, "RW", fillColor, ControlDash, "black");
            CreateNode("MTR2", 1055, 108, 35, 15, 0, 0, MTR2Ports, FlowShapeType.Process, "MTR", fillColor, ControlDash, "black");

            CreateNode("FlgReturn", 750, 10, 1, 1, 0, 0, FLRetPorts, FlowShapeType.Process, "", fillColor, "8,0", "black");

            // Write Nodes
            CreateNode("WriteMux", 1125, 315, 60, 27, -90, 90, WriteMuxPorts, FlowShapeType.Terminator, "Mux", fillColor, "8,0", "black");
            CreateNode("RdRegReturn", 750, 635, 1, 1, 0, 0, RdRegReturnPorts, FlowShapeType.Process, "", fillColor, "8,0", "black");
            CreateNode("RWReturn", 750, 15, 1, 1, 0, 0, RWRetPorts, FlowShapeType.Process, "", fillColor, "8,0", "black");

            // Window Sizing Node
            CreateNode("sizeNodeYX", 1170, 650, 1, 1, 0, 0, WinSizePorts, FlowShapeType.Process, "", fillColor, "8,0", fillColor);
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

            UpdateStaticDiagram();
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
                                     AnnotationAlignment align = AnnotationAlignment.Before, double offset = 1, PathAnnotation pAnnotate = null, OrthogonalSegment segment1 = null, OrthogonalSegment segment2 = null, DecoratorShape decoratorShape = DecoratorShape.Arrow)
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
                    Style = new ShapeStyle() { StrokeColor = strokeColor, Fill = strokeColor },
                    Shape = decoratorShape
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
                RotationAngle = rAngleAnnotation,
                Width = xSize
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
            UpdateCounters();
        }
        private void DialogClose(Object args)
        {
            debugRender = true;
            diagramContent = diagram.SaveDiagram();
            this.ShowButton = false;
            debugRender = true;
            UpdateCounters();
        }
        private void OnClicked()
        {
            if (StaticMode == true)
            {
                InitStaticDiagramModel();
                StaticVisibility = true;
            }    
            else if (StaticMode == false)
            {
                InitDynamicDiagramModel();
                DynamicVisibility = true;
            }
        }

        private void OnClickedCache()
        {
            ShowCache = !ShowCache;
        }

        private void errorClose(Object args)
        {
            this.errorVis = false;
        }




        // Cache Data

        public record CacheLineData(string Index, string Tag, string Data);

        public class InstructionCacheL1
        {
            public string? Index { get; set; }
            public string? Tag { get; set; }
            public string? DataAtAddress { get; set; }

            public static List<InstructionCacheL1> GetL1CacheData()
            {
                List<InstructionCacheL1> L1CacheData = new List<InstructionCacheL1>();
                L1CacheData.Add(new InstructionCacheL1 { Index = "0x00", Tag = "0x00", DataAtAddress = "ADD r1, r2, r3" });
                L1CacheData.Add(new InstructionCacheL1 { Index = "0x00", Tag = "0x01", DataAtAddress = "SUB r1, r2, r3" });
                L1CacheData.Add(new InstructionCacheL1 { Index = "0x00", Tag = "0x02", DataAtAddress = "ADD r1, r2, r3" });
                L1CacheData.Add(new InstructionCacheL1 { Index = "0x00", Tag = "0x03", DataAtAddress = "SUB r1, r2, r3" });
                L1CacheData.Add(new InstructionCacheL1 { Index = "0x00", Tag = "0x04", DataAtAddress = "ADD r1, r2, r3" });
                L1CacheData.Add(new InstructionCacheL1 { Index = "0x00", Tag = "0x05", DataAtAddress = "ADD r1, r2, r3" });
                L1CacheData.Add(new InstructionCacheL1 { Index = "0x01", Tag = "0x00", DataAtAddress = "SUB r1, r2, r3" });
                L1CacheData.Add(new InstructionCacheL1 { Index = "0x01", Tag = "0x01", DataAtAddress = "SUB r1, r2, r3" });
                L1CacheData.Add(new InstructionCacheL1 { Index = "0x01", Tag = "0x02", DataAtAddress = "SUB r1, r2, r3" });
                L1CacheData.Add(new InstructionCacheL1 { Index = "0x01", Tag = "0x03", DataAtAddress = "ADD r1, r2, r3" });
                L1CacheData.Add(new InstructionCacheL1 { Index = "0x01", Tag = "0x04", DataAtAddress = "ADD r1, r2, r3" });
                L1CacheData.Add(new InstructionCacheL1 { Index = "0x01", Tag = "0x05", DataAtAddress = "ADD r1, r2, r3" });


                return L1CacheData;
            }
        }

        public class InstructionCacheL2
        {
            public string? Index { get; set; }
            public string? Tag { get; set; }
            public string? DataAtAddress { get; set; }

            public static List<InstructionCacheL2> GetL2CacheData()
            {
                List<InstructionCacheL2> L2CacheData = new List<InstructionCacheL2>();
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x00", Tag = "0x00", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x00", Tag = "0x01", DataAtAddress = "SUB r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x00", Tag = "0x02", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x00", Tag = "0x03", DataAtAddress = "SUB r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x00", Tag = "0x04", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x00", Tag = "0x05", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x01", Tag = "0x00", DataAtAddress = "SUB r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x01", Tag = "0x01", DataAtAddress = "SUB r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x01", Tag = "0x02", DataAtAddress = "SUB r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x01", Tag = "0x03", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x01", Tag = "0x04", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x01", Tag = "0x05", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x02", Tag = "0x00", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x02", Tag = "0x01", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x02", Tag = "0x02", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x02", Tag = "0x03", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x02", Tag = "0x04", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x02", Tag = "0x05", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x03", Tag = "0x00", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x03", Tag = "0x01", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x03", Tag = "0x02", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x03", Tag = "0x03", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x03", Tag = "0x04", DataAtAddress = "ADD r1, r2, r3" });
                L2CacheData.Add(new InstructionCacheL2 { Index = "0x03", Tag = "0x05", DataAtAddress = "ADD r1, r2, r3" });

                return L2CacheData;
            }
        }

        public void expandL1Cache()
        {
            if (L1CacheExpandToggle == false)
            {
                _ = L1CacheGridObject.CollapseAllGroupAsync();
                L1CacheExpandToggle = true;
            }
            else
            {
                _ = L1CacheGridObject.ExpandAllGroupAsync();
                L1CacheExpandToggle= false;
            }
            
        }

        public void expandL2Cache()
        {
            if (L2CacheExpandToggle == false)
            {
                _ = L2CacheGridObject.CollapseAllGroupAsync();
                L2CacheExpandToggle = true;
            }
            else
            {
                _ = L2CacheGridObject.ExpandAllGroupAsync();
                L2CacheExpandToggle = false;
            }
        }



        // Monaco Editor

        private StandaloneEditorConstructionOptions EditorConstructionOptions(MonacoEditor editor)
        {
            return new StandaloneEditorConstructionOptions
            {
                AutomaticLayout = true,
                Language = "ISInstructionSet",
                Value = ""
            };
        }

        private async Task EditorOnDidInit(MonacoEditorBase editor)
        {
            var newDecorations = new ModelDeltaDecoration[]
            {
                new ModelDeltaDecoration
                {
                    Range = new BlazorMonaco.Range(3,1,3,1),
                    Options = new ModelDecorationOptions
                    {
                        IsWholeLine = true,
                        ClassName = "decorationContentClass",
                        GlyphMarginClassName = "decorationGlyphMarginClass"
                    }
                }
            };

            await MonacoEditorBase.DefineTheme("ISTheme", new StandaloneThemeData
            {
                Base = FrontendVariables.darkMode ? "vs-dark" : "vs",
                Inherit = false,
                Rules = new List<TokenThemeRule>
                {
                    new TokenThemeRule { Background = (FrontendVariables.darkMode ? "000000" : "FFFFFF"), Foreground = (FrontendVariables.darkMode ? "E0E0E0" : "000000")},
                    new TokenThemeRule { Token = "mnemonic", Foreground = (FrontendVariables.darkMode ? "4353FA" : "0524a3") },
                    new TokenThemeRule { Token = "register", Foreground = (FrontendVariables.darkMode ? "999900" : "777700") },
                    new TokenThemeRule { Token = "comment", Foreground = (FrontendVariables.darkMode ? "119922" : "11AA22"), FontStyle = "italic" },
                    new TokenThemeRule { Token = "addressModes", Foreground = (FrontendVariables.darkMode ? "FF7DA4" : "94072d") },
                    new TokenThemeRule { Token = "branchLabel", Foreground = (FrontendVariables.darkMode ? "7E5EFF" : "4b0774") },
                },
                Colors = new Dictionary<string, string>
                {
                    ["editor.background"] = (FrontendVariables.darkMode ? "#000000" : "#FFFFFF"),
                    ["editorCursor.foreground"] = (FrontendVariables.darkMode ? "#E0E0E0" : "#000000"),
                    ["editorLineNumber.foreground"] = "#7A7A7A"
                }
            });

            await MonacoEditorBase.SetTheme("ISTheme");

            if (ExecAssemblyCode != "")
            {
                await _editor.SetValue(ExecAssemblyCode);
            }

            await JSRuntime.InvokeVoidAsync("setupMonacoLanguage");
        }

    }
}