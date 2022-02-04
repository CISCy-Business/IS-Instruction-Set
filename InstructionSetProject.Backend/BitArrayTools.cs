using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend
{
    internal static class BitArrayTools
    {
        public static BitArray Append(BitArray first, BitArray second)
        {
            var newArr = new bool[first.Count + second.Count];
            first.CopyTo(newArr, 0);
            second.CopyTo(newArr, first.Count);
            return new BitArray(newArr);
        }
    }
}
