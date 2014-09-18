using System;
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
            Memory ram = null;
            uint startPoint = Loader.PreloadRAM(arguments, ref ram);
            Debug.Assert(ram.ComputeMD5() == "3500a8bef72dfed358b25b61b7602cf1");
            Console.WriteLine("Loader: Testing: All Loader Tests Passed!");
        }
    }
}
