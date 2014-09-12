using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ARMSim_2._0
{
    class TestOptions
    {
        // Runs Unit Tests for Options Class
        public void RunTests ()
        {
            Console.WriteLine("Loader: Tests: Options: Testing Options");
            Options op = new Options(new[] {"--load", "test1.exe", "--mem", "100000", "--test"});
            Debug.Assert(op.fileName == "test1.exe" && op.memorySize == 100000 && op.testMode == true);
            op = new Options(new[] {"--load", "test1.exe", "--mem", "100000", "--mem", "crash"});
            Debug.Assert(op.fileName == "");
            Console.WriteLine("Loader: Tests: Options: All Tests Passed");
        }
    }
}
