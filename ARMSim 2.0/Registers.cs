using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    public class Registers : Memory
    {
        public Registers(uint EntryPoint) : base(64)
        {
            WriteRegister(15, EntryPoint);
        }

        // Reads register number <registerNumber> (zero-based)
        public uint ReadRegister(uint registerNumber)
        {
            if (registerNumber < 16)
            {
                return ReadWord(registerNumber * 4);
            }
            else
            {
                return 0;
            }
        }

        // Writes <data> to register number <registerNumber> (zero-based)
        public void WriteRegister(uint registerNumber, uint data)
        {
            if (registerNumber < 16)
            {
                WriteWord(registerNumber * 4, data);
            }
        }

        public void IncrementProgramCounter()
        {
            WriteRegister(15, ReadRegister(15) + 4);
        }
    }
}
