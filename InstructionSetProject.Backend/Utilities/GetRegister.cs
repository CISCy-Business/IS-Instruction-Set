using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Utilities
{
    public static class GetRegister
    {
        public static string FromByte(byte code)
        {
            switch (code)
            {
                case 0:
                    return "r0";
                case 1:
                    return "r1";
                case 2:
                    return "r2";
                case 3:
                    return "r3";
                case 4:
                    return "r4";
                case 5:
                    return "r5";
                case 6:
                    return "r6";
                case 7:
                    return "r7";
                default:
                    throw new Exception("Unknown register value passed");
            }
        }

        public static byte FromString(string register)
        {
            switch (register)
            {
                case "r0":
                    return 0;
                case "r1":
                    return 1;
                case "r2":
                    return 2;
                case "r3":
                    return 3;
                case "r4":
                    return 4;
                case "r5":
                    return 5;
                case "r6":
                    return 6;
                case "r7":
                    return 7;
                default:
                    throw new Exception("Unknown register value passed");
            }
        }
    }
}
