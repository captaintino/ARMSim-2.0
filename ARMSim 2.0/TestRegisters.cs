using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ARMSim_2._0
{
    class TestRegisters
    {
        // Runs Unit Tests for Registers Class
        public void RunTests()
        {
            Console.WriteLine("Prototype: Tests: Registers: Testing Registers");
            Registers regs = new Registers(0);
            Debug.Assert(regs.ReadRegister(15) == 0);
            regs.WriteRegister(15, 100);
            Debug.Assert(regs.ReadRegister(15) == 100);
            Console.WriteLine("Prototype: Tests: Registers: Read/Write Passed");
            Console.WriteLine("Prototype: Tests: Registers: All tests Passed");
        }
    }
}
