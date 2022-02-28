using InstructionSetProject.Backend.StaticPipeline;
using InstructionSetProject.Backend;
using InstructionSetProject.Backend.StaticFrontend;
using Syncfusion.Blazor.Diagram;
using System.Collections.ObjectModel;
using System.Text;
using InstructionSetProject.Backend.Execution;
using InstructionSetProject.Backend.InstructionTypes;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Popups;
using DiagramShapes = Syncfusion.Blazor.Diagram.Shapes;
using DiagramSegments = Syncfusion.Blazor.Diagram.ConnectorSegmentType;

namespace InstructionSetProject.Frontend.Pages
{
    public partial class ExecuterPage
    {
        private string ExecMachineCode = "";
        private string ExecAssemblyCode = "";
        private string statsString = "";

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
        public string IP => SPEx != null ? SPEx.DataStructures.InstructionPointer.value.ToString("X4") : "0000";
        public string SP => SPEx != null ? SPEx.DataStructures.StackPointer.value.ToString("X4") : "0000";
        public string FL => SPEx != null ? SPEx.DataStructures.Flags.value.ToString("X4") : "0000";
        public string PC => SPEx != null ? SPEx.DataStructures.InstructionPointer.value.ToString("X4") : "0000";

        private bool debugRender = false;

        private int connectorCount = 0;
        // Reference to diagram
        SfDiagramComponent diagram;
        // Defines diagram's nodes collection
        public DiagramObjectCollection<Node> NodeCollection { get; set; }
        // Defines diagram's connector collection
        public DiagramObjectCollection<Connector> ConnectorCollection { get; set; }


        private List<byte> machineCode = new();
        private string output { get; set; }
        private string machineCodeString { get; set; }

        private string fileContent = "";

        public bool ShowItem { get; set; } = true;
        private bool Visibility { get; set; } = false;
        private bool errorVis { get; set; } = false;
        private bool ShowButton { get; set; } = false;
        private ResizeDirection[] dialogResizeDirections { get; set; } = new ResizeDirection[] { ResizeDirection.All };

        private string MemoryBytesUpdate(byte[] bytes)
        {
            if (bytes != null)
            {
                return String.Join(" ", bytes);
            }
            else
            {
                return "No Bytes To Display in Memory";
            }
        }

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
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

        protected override async Task OnInitializedAsync()
        {
            StartupMethod();
            InitDiagramModel();
        }

        void StartupMethod()
        {
            ExecMachineCode = FrontendVariables.currentMachineCodeExecuter;
            FrontendVariables.currentMachineCodeExecuter = "";
            ExecAssemblyCode = FrontendVariables.currentAssemblyCodeExecuter;
            FrontendVariables.currentAssemblyCodeExecuter = "";
        }

        private static List<byte> HexStringToByteList(string machineCodeString)
        {
            if (machineCodeString.Length % 2 == 1)
                throw new Exception("Cannot have an odd number of digits!!");

            int numChars = machineCodeString.Length;
            byte[] bytes = new byte[numChars / 2];
            for (int i = 0; i < numChars; i += 2)
                bytes[i / 2] = Convert.ToByte(machineCodeString.Substring(i, 2), 16);

            List<byte> mCode = new List<byte>(bytes);

            return mCode;
        }

        void buildCode()
        {
            if (ExecAssemblyCode.Length != 0)
            {
                machineCode = Assembler.Assemble(ExecAssemblyCode);
                string hexCode = BitConverter.ToString(machineCode.ToArray());
                ExecMachineCode = hexCode.Replace("-", " ");
            }
            else
            {
                errorVis = true;
            }
        }

        void runCode()
        {
            SPEx = (StaticPipelineExecution)StaticPipelineExecutor.Execute(ExecAssemblyCode);
            SPEx.Continue();
        }

        void Debug()
        {
            SPEx = (StaticPipelineExecution)StaticPipelineExecutor.Execute(ExecAssemblyCode);
            OnItemClick();
            debugRender = true;
            JSRuntime.InvokeVoidAsync("debugScrollToTop");
        }

        bool IsSelectedFetch(IInstruction instr) => instr == SPEx.fetchingInstruction;
        bool IsSelectedDecode(IInstruction instr) => instr == SPEx.decodingInstruction;
        bool IsSelectedExecute(IInstruction instr) => instr == SPEx.executingInstruction;
        bool IsSelectedMemory(IInstruction instr) => instr == SPEx.memoryInstruction;
        bool IsSelectedWrite(IInstruction instr) => instr == SPEx.writingBackInstruction;

        string DivCSS(IInstruction instr) => IsSelectedFetch(instr) ? "bg-fetch text-white" : (IsSelectedDecode(instr) ? "bg-decode text-white" : (IsSelectedExecute(instr) ? "bg-execute text-white" : (IsSelectedMemory(instr) ? "bg-memory text-white" : (IsSelectedWrite(instr) ? "bg-write text-white" : "bg-white"))));

        void step()
        {
            debugRender = true;
            SPEx.Step();
            JSRuntime.InvokeVoidAsync("stepScroll");
            debugRender = true;
        }

        void Continue()
        {
            SPEx.Continue();
        }

        private void InitDiagramModel()
        {
            NodeCollection = new DiagramObjectCollection<Node>();
            ConnectorCollection = new DiagramObjectCollection<Connector>();

            #region Ports
            List<PointPort> FetchMuxPorts = new List<PointPort>();
            FetchMuxPorts.Add(AddPort("portFetchMuxIn0", 0.15, 0.01));
            FetchMuxPorts.Add(AddPort("portFetchMuxIn1", 0.85, 0.01));
            FetchMuxPorts.Add(AddPort("portFetchMuxOut0", 0.5, 1));
            List<PointPort> PCPorts = new List<PointPort>();
            PCPorts.Add(AddPort("portPCIn", 0.01, 0.5));
            PCPorts.Add(AddPort("portPCOut", 1, 0.5));
            List<PointPort> InstrMemPorts = new List<PointPort>();
            InstrMemPorts.Add(AddPort("portInstrMemIn", 0.01, 0.1));
            InstrMemPorts.Add(AddPort("portInstrMemOut", 1, 0.5));
            List<PointPort> AddPCPorts = new List<PointPort>();
            AddPCPorts.Add(AddPort("portAddPCIn0", 0.15, 0.01));
            AddPCPorts.Add(AddPort("portAddPCIn1", 0.85, 0.01));
            AddPCPorts.Add(AddPort("portAddPCOut0", 0.5, 1));
            List<PointPort> ifidPorts = new List<PointPort>();
            ifidPorts.Add(AddPort("portIfidIn0", 0.01, 0.30));
            ifidPorts.Add(AddPort("portIfidIn1", 0.01, 0.5));
            ifidPorts.Add(AddPort("portIfidOut0", 1, 0.15));
            ifidPorts.Add(AddPort("portIfidOut1", 1, 0.5));

            // Window Sizing Ports
            List<PointPort> WinSizePorts = new List<PointPort>();
            #endregion

            #region Nodes
            CreateNode("FetchMux", 60, 233, 60, 27, -90, 90, FetchMuxPorts, FlowShapeType.Terminator, "Mux", "white", "black");
            CreateNode("PC", 110, 233, 25, 55, 0, 0, PCPorts, FlowShapeType.Process, "PC", "white", "black");
            CreateNode("InstrMem", 200, 273, 100, 100, 0, 0, InstrMemPorts, FlowShapeType.Process, "Instruction Memory", "white", "black");
            CreateNode("AddPC", 200, 143, 75, 50, -90, 90, AddPCPorts, BasicShapeType.Trapezoid, "Add", "white", "black");
            CreateNode("IFID", 300, 273, 30, 400, 0, -90, ifidPorts, FlowShapeType.Process, "IF/ID", "white", "black");

            // Window Sizing Node
            CreateNode("sizeNodeYX", 500, 500, 1, 1, 0, 0, WinSizePorts, FlowShapeType.Process, "", "white", "white");
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
            CreateConnector("FetchMux", "portFetchMuxOut0", "PC", "portPCIn");
            CreateConnector("PC", "portPCOut", "InstrMem", "portInstrMemIn");
            CreateConnector("PC", "portPCOut", "AddPC", "portAddPCIn1");
            CreateConnector("AddPC", "portAddPCOut0", "FetchMux", "portFetchMuxIn1");
            CreateConnector("PC", "portPCOut", "IFID", "portIfidIn0");
            CreateConnector("InstrMem", "portInstrMemOut", "IFID", "portIfidIn1");
            #endregion
        }

        private void CreateConnector(string sourceId, string sourcePortId, string targetId, string targetPortId, string label = default(string), OrthogonalSegment segment1 = null, OrthogonalSegment segment2 = null)
        {
            Connector diagramConnector = new Connector()
            {
                ID = string.Format("connector{0}", ++connectorCount),
                SourceID = sourceId,
                SourcePortID = sourcePortId,
                TargetID = targetId,
                TargetPortID = targetPortId
            };

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
                    VerticalAlignment = VerticalAlignment.Bottom
                };
                diagramConnector.Annotations = new DiagramObjectCollection<PathAnnotation>() { annotation };
            }

            ConnectorCollection.Add(diagramConnector);
        }

        private PointPort AddPort(string id, double x, double y)
        {
            return new PointPort()
            {
                ID = id,
                Shape = PortShapes.Circle,
                Width = 3,
                Height = 3,
                Visibility = PortVisibility.Visible,
                Offset = new DiagramPoint() { X = x, Y = y },
                Style = new ShapeStyle() { Fill = "#1916C1", StrokeColor = "#000" },
                Constraints = PortConstraints.Default | PortConstraints.Draw
            };
        }

        private void CreateNode(string id, double xOffset, double yOffset, int xSize, int ySize, int rAngleNode, int rAngleAnnotation,
            List<PointPort> ports, FlowShapeType shape, string label, string fillColor, string stroke)
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
                Color = "black",
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
                Style = new ShapeStyle() { Fill = fillColor, StrokeColor = stroke },
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

        private void CreateNode(string id, double xOffset, double yOffset, int xSize, int ySize, int rAngleNode, int rAngleAnnotation,
            List<PointPort> ports, BasicShapeType shape, string label, string fillColor, string stroke)
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
                Color = "black",
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
                Style = new ShapeStyle() { Fill = fillColor, StrokeColor = stroke },
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

        private void DialogOpen(Object args)
        {
            this.ShowButton = true;
        }
        private void DialogClose(Object args)
        {
            this.ShowButton = false;
        }
        private void OnClicked()
        {
            this.Visibility = true;
        }

        private void errorClose(Object args)
        {
            this.errorVis = false;
        }

    }
}
