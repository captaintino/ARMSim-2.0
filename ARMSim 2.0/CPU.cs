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

        public CPU(Options arguments)
        {
            ram = Loader.PreloadRAM(arguments);
            registers = new Registers();
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


    }
}
