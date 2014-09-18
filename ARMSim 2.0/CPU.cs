using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    class CPU
    {
        public Memory ram;
        public Registers registers;
        bool n, z, c, f;

        public CPU(Options arguments)
        {
            uint entryPoint = Loader.PreloadRAM(arguments, ref ram);
            registers = new Registers(entryPoint);
            n = z = c = f = false;
        }

        public uint fetch(uint address)
        {
            return ram.ReadWord(address);
        }

        public uint /*???*/ decode()
        {
            return 0;
        }

        public void execute()
        {
            System.Threading.Thread.Sleep(250);
        }

        public string FlagsToString()
        {
            return (n ? "1" : "0") + (z ? "1" : "0") + (c ? "1" : "0") + (f ? "1" : "0"); 
        }

    }
}
