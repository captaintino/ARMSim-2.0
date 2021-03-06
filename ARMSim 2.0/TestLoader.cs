﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ARMSim_2._0
{
    class TestLoader
    {
        // Runs Unit Tests for Loader Class
        public void RunTests()
        {
            Console.WriteLine("Loader: Testing: Preloading RAM");
            Console.WriteLine("Loader: Testing: Using file = test1.exe");
            Options arguments = new Options(new[] { "--load", "test1.exe" });
            CPU cpu = Loader.PreloadCPU(arguments, null);
            Debug.Assert(cpu.ram.ComputeMD5() == "161167163026d89e540b508fb1bb9847"); // Lost old test1 file, updated MD5
            Console.WriteLine("Loader: Testing: All Loader Tests Passed!");
        }
    }
}
