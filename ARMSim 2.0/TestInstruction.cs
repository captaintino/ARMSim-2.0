using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ARMSim_2._0
{
    class TestInstruction
    {
        public void RunTests()
        {
            Console.WriteLine("Simulator: Tests: Instruction: Testing Instructions");
            Registers regs = new Registers(0);
            Memory ram = new Memory();
            regs.WriteRegister(3, 100);
            Console.WriteLine("Simulator: Tests: Instruction: Testing Data Processing Instructions");
            Console.WriteLine("Simulator: Tests: Instruction: Testing MOV Instruction");
            DPInstruction dp = new DPInstruction(0xE1A05003u);
            dp.decode();
            Debug.Assert(dp.ToString() == "MOV R5, R3");
            dp.execute(regs, ram);
            Debug.Assert(regs.ReadRegister(5) == 100);
            Console.WriteLine("Prototype: Tests: CPU: Memory Access tests Passed");
            Console.WriteLine("Simulator: Tests: Instruction: All tests Passed");
        }
    }
}
