using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Utilities
{
    public static class AddressingMode
    {
        private static Dictionary<ushort, string> m_map = new()
        {
            { 0b000_0000, "i" },
            { 0b000_1000, "id" },
            { 0b001_0000, "in" },
            { 0b001_1000, "rd" },
            { 0b010_0000, "rn" },
            { 0b010_1000, "xd" },
            { 0b011_0000, "xn" },
            { 0b011_1000, "xo" },
            { 0b100_0000, "xf" },
            { 0b100_1000, "sd" },
            { 0b101_0000, "sn" },
            { 0b101_1000, "so" },
            { 0b110_0000, "sxd" },
            { 0b110_1000, "sxn" },
            { 0b111_0000, "sxo" },
            { 0b111_1000, "sxf" }
        };

        public static ushort Get(string modeName)
        {
            return m_map.FirstOrDefault(mode => mode.Value == modeName).Key;
        }

        public static string Get(ushort modeId)
        {
            return m_map[modeId];
        }
    }
}
