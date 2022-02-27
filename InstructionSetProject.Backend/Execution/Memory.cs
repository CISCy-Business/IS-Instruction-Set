using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstructionSetProject.Backend.Execution
{
    public class Memory
    {
        public byte[] Bytes = new byte[1048576];

        public void Write(ushort address, ushort value)
        {

        }

        public ushort Read(ushort address)
        {
            throw new NotImplementedException();
        }
    }
}
