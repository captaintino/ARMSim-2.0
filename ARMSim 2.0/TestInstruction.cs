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
            regs.WriteRegister(4, 1);
            Console.WriteLine("Simulator: Tests: Instruction: Testing Data Processing Instructions");

            Console.WriteLine("Simulator: Tests: Instruction: Testing MOV Instruction");
            DPInstruction dp = new DPInstruction(0xE1A05003u);
            dp.decode();
            Debug.Assert(dp.ToString() == "mov r5, r3");
            dp.execute(regs, ram);
            Debug.Assert(regs.ReadRegister(5) == 100);

            dp = new DPInstruction(0xE1A05083u);
            dp.decode();
            Debug.Assert(dp.ToString() == "mov r5, r3, lsl #1");
            dp.execute(regs, ram);
            Debug.Assert(regs.ReadRegister(5) == 200);

            dp = new DPInstruction(0xE3A05180u);
            dp.decode();
            Debug.Assert(dp.ToString() == "mov r5, #32");
            dp.execute(regs, ram);
            Debug.Assert(regs.ReadRegister(5) == 32);

            dp = new DPInstruction(0xE1A05413u);
            dp.decode();
            Debug.Assert(dp.ToString() == "mov r5, r3, lsl r4");
            dp.execute(regs, ram);
            Debug.Assert(regs.ReadRegister(5) == 200);
            Console.WriteLine("Simulator: Tests: Instruction: MOV Instruction Test Passed");

            Console.WriteLine("Simulator: Tests: Instruction: All tests Passed");
        }
    }
}
