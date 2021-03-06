﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ARMSim_2._0
{
    class TestMemory
    {
        // Runs Unit Tests for Memory Class
        public void RunTests ()
        {
            Console.WriteLine("Loader: Tests: RAM: Testing RAM");
            Memory ram = new Memory();
            ram.WriteWord(1000u, 1111u);
            Debug.Assert(ram.ReadWord(1000u) == 1111u);
            ram.WriteHalfWord(1020u, 11u);
            Debug.Assert(ram.ReadHalfWord(1020u) == 11u);
            ram.WriteHalfWord(1041u, 11u);
            Debug.Assert(ram.ReadHalfWord(1041u) == 0u);
            ram.WriteByte(500u, (byte)23u);
            Debug.Assert(ram.ReadByte(500u) == (byte)23u);
            ram.WriteWord(0, uint.MaxValue);
            Console.WriteLine("Loader: Tests: RAM: Read/Write Passed");
            for (int i = 0; i < 32; ++i)
            {
                Debug.Assert(ram.TestFlag(0, i) == true);
            }
            ram.SetFlag(20u, 2, true);
            Debug.Assert(ram.TestFlag(20u, 2) == true);
            ram.SetFlag(20u, 2, false);
            Debug.Assert(ram.TestFlag(20u, 2) == false);
            Console.WriteLine("Loader: Tests: RAM: Test/SetFlag Passed");
            Console.WriteLine("Loader: Tests: RAM: All tests Passed");
        }
    }
}
