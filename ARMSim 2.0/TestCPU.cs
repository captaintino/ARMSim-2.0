using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ARMSim_2._0
{
    class TestCPU
    {
        public void RunTests()
        {
            Console.WriteLine("Prototype: Tests: CPU: Testing CPU");
            Registers regs = new Registers(150);
            Memory ram = new Memory();
            CPU cpu = new CPU(ram, regs, null);
            Debug.Assert(cpu.GetNFlag() == false && cpu.GetZFlag() == false && cpu.GetCFlag() == false && cpu.GetFFlag() == false);
            Debug.Assert(cpu.FlagsToString() == "0000");
            Console.WriteLine("Prototype: Tests: CPU: Flag tests Passed");
            Debug.Assert(cpu.GetStackPointer() == 0);
            Debug.Assert(cpu.GetRegister(15) == 150);
            Debug.Assert(cpu.GetWord(2000) == 0);
            Console.WriteLine("Prototype: Tests: CPU: Memory Access tests Passed");
            Console.WriteLine("Prototype: Tests: CPU: All tests Passed");
        }
    }
}
