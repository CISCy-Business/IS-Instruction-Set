using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstructionSetProject.Backend.Instructions;
using InstructionSetProject.Backend.InstructionTypes;
using InstructionSetProject.Backend.Utilities;

namespace InstructionSetProject.Backend
{
    public static class Disassembler
    {
        public static int progCounter = 0;
        public static string programCounter = "";
        public static bool immOrJump = false;
        public static string addrMode = "";
        public static string assemblyCode = "";
        public static int totalInstructions { get; set; } = 0;
        public static int arithmeticInstructions { get; set; } = 0;
        public static int generalInstructions { get; set; } = 0;
        public static int jumpInstructions { get; set; } = 0;
        public static int stackInstructions { get; set; } = 0;
        public static int immediateAddrMode { get; set; } = 0;
        public static int directAddrMode { get; set; } = 0;
        public static int indirectAddrMode { get; set; } = 0;
        public static int registerDirectAddrMode { get; set; } = 0;
        public static int registerIndirectAddrMode { get; set; } = 0;
        public static int indexDirectAddrMode { get; set; } = 0;
        public static int indexIndirectAddrMode { get; set; } = 0;
        public static int indexOffsetAddrMode { get; set; } = 0;
        public static int indexDefferedAddrMode { get; set; } = 0;
        public static int stackDirectAddrMode { get; set; } = 0;
        public static int stackIndirectAddrMode { get; set; } = 0;
        public static int stackOffsetAddrMode { get; set; } = 0;
        public static int stackIndexDirectAddrMode { get; set; } = 0;
        public static int stackIndexIndirectAddrMode { get; set; } = 0;
        public static int stackIndexOffsetAddrMode { get; set; } = 0;
        public static int stackIndexDefferedAddrMode { get; set; } = 0;

        public static string Disassemble(List<byte> machineCode)
        {
            var instructionCode = "";
            assemblyCode = "";
            totalInstructions = 0;
            arithmeticInstructions = 0;
            generalInstructions = 0;
            jumpInstructions = 0;
            stackInstructions = 0;
            immediateAddrMode = 0;
            directAddrMode = 0;
            indirectAddrMode = 0;
            registerDirectAddrMode = 0;
            registerIndirectAddrMode = 0;
            indexDirectAddrMode = 0;
            indexIndirectAddrMode = 0;
            indexOffsetAddrMode = 0;
            indexDefferedAddrMode = 0;
            stackDirectAddrMode = 0;
            stackIndirectAddrMode = 0;
            stackOffsetAddrMode = 0;
            stackIndexDirectAddrMode = 0;
            stackIndexIndirectAddrMode = 0;
            stackIndexOffsetAddrMode = 0;
            stackIndexDefferedAddrMode = 0;

            for (int i = 0; i < machineCode.Count; i += 2)
            {
                immOrJump = false;
                var instruction = new List<byte>();
                instruction.Add(machineCode[i]);
                instruction.Add(machineCode[i + 1]);

                string instOpCode = machineCode[i].ToString("X2");
                string instOperand = machineCode[i + 1].ToString("X2");

                progCounter = i;

                var instrType = InstructionUtilities.GetInstructionType(instruction);
                if (instrType == InstructionType.Memory || instrType == InstructionType.Jump || instrType == InstructionType.R2I)
                {
                    instruction.Add(machineCode[i + 2]);
                    instruction.Add(machineCode[i + 3]);
                    instOperand += machineCode[i + 2].ToString("X2");
                    instOperand += machineCode[i + 3].ToString("X2");
                    immOrJump = true;
                    i += 2;
                }

                if (progCounter.ToString("X2").Length == 2)
                {
                    programCounter = "00" + progCounter.ToString("X2");
                }
                else if (progCounter.ToString("X2").Length == 3)
                {
                    programCounter = "0" + progCounter.ToString("X2");
                }
                else
                {
                    programCounter = progCounter.ToString("X2");
                }

                string fullInstruction = ConvertLineToAssemblyCode(instruction);

                if (instrType == InstructionType.Memory)
                {
                    instructionCode += "   " + programCounter + "      ";
                    instructionCode += instrType.ToString().Substring(0, 1).ToLower() + "       ";
                    instructionCode += instOpCode + "       ";
                    instructionCode += fullInstruction;
                   

                    // switch (addrMode)
                    // {
                    //     case "i": immediateAddrMode++; break;
                    //     case "d": directAddrMode++; break;
                    //     case "dn": indirectAddrMode++; break;
                    //     case "r": registerDirectAddrMode++; break;
                    //     case "rn": registerIndirectAddrMode++; break;
                    //     case "xd": indexDirectAddrMode++; break;
                    //     case "xn": indexIndirectAddrMode++; break;
                    //     case "xo": indexOffsetAddrMode++; break;
                    //     case "xf": indexDefferedAddrMode++; break;
                    //     case "sd": stackDirectAddrMode++; break;
                    //     case "sn": stackIndirectAddrMode++; break;
                    //     case "so": stackOffsetAddrMode++; break;
                    //     case "sxd": stackIndexDirectAddrMode++; break;
                    //     case "sxn": stackIndexIndirectAddrMode++; break;
                    //     case "sxo": stackIndexOffsetAddrMode++; break;
                    //     case "sxf": stackIndexDefferedAddrMode++; break;
                    //     default:
                    //         throw new Exception($"Address Not Found: {addrMode}");
                    // }

                    instructionCode += "\n";
                }
                else if (instrType == InstructionType.Jump)
                {
                    instructionCode += "   " + programCounter + "      ";
                    instructionCode += instrType.ToString().Substring(0, 1).ToLower() + "       ";
                    instructionCode += instOpCode + "       ";
                    instructionCode += fullInstruction;
                    instructionCode += "\n";
                }
                else
                {
                    instructionCode += "   " + programCounter + "      ";
                    instructionCode += instrType.ToString() + "      ";
                    instructionCode += instOpCode + "       ";
                    instructionCode += fullInstruction;
                    instructionCode += "\n";
                }

                assemblyCode += fullInstruction + "\n";

                totalInstructions++;

                if (instrType.ToString().Substring(0, 2).ToLower().Equals("r2") || instrType.ToString().Substring(0, 2).ToLower().Equals("r3"))
                {
                    arithmeticInstructions++;
                }

                if (instOpCode.Substring(0, 1).Equals("8") || instOpCode.Equals("00"))
                {
                    generalInstructions++;
                }

                if (instrType.ToString().Substring(0, 1).ToLower().Equals("j"))
                {
                    jumpInstructions++;
                }

                if (instrType.ToString().Substring(0, 2).ToLower().Equals("r1"))
                {
                    stackInstructions++;
                }
                
            }

            progCounter = 0;
            return instructionCode.TrimEnd();
        }

        public static string ConvertLineToAssemblyCode(List<byte> instruction)
        {
            var beginInstruction = (ushort)(instruction[0] << 8);
            beginInstruction += instruction[1];

            var instr = InstructionManager.Instance.Get(InstructionUtilities.GetOpCode(beginInstruction));
            instr.ParseInstruction(instruction);
            return instr.Disassemble();
        }
    }
}
