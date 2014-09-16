using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    class Computer
    {
        CPU cpu;
        bool stop = true;

        public Computer(Options arguments)
        {
            cpu = new CPU(arguments);
        }

        public void stopRun()
        {
            stop = true;
        }

        public void run()
        {
            stop = false;
            uint num;

            do
            {
                // rewrite
                num = cpu.fetch(4u);
                cpu.decode();
                cpu.execute();
                num = 1;
            } while (num != 0 && !stop);
        }

        public void step()
        {
            // rewrite
            uint num = cpu.fetch(4u);
            cpu.decode();
            cpu.execute();
        }

        public string getMD5()
        {
            return cpu.ram.ComputeMD5();
        }

    }
}
