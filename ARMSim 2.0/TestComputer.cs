using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ARMSim_2._0
{
    class TestComputer
    {
        public void RunTests()
        {
            Console.WriteLine("Prototype: Tests: Computer: Testing Computer");
            Options arguments = new Options(new[] { "--load", "test1.exe" });
            FileStream traceFile = new FileStream("trace.log", FileMode.Create, FileAccess.Write);
            Computer computer = new Computer(arguments, ref traceFile);

            List<bool> test1 = computer.GetFlags();
            List<bool> test2 = new List<bool>(new[] { false, false, false, false });
            Debug.Assert(test1[0] == test2[0] && test1[1] == test2[1] && test1[2] == test2[2] && test1[3] == test2[3]);
            Debug.Assert(computer.GetMD5() == "3500a8bef72dfed358b25b61b7602cf1");
            Debug.Assert(computer.GetStackPointer() == 0x7000);
            List<uint> test3 = new List<uint>(new[] { 0u, 0u, 0u, 0u, 0u, 0u, 0u, 0u, 0u, 0u, 0u, 0u, 0u, 0x7000u, 0u, 0x138u });
            List<uint> test4 = computer.GetRegisters();
            for (int i = 0; i < 16; ++i){
                Debug.Assert(test3[i] == test4[i] - (i == 15 ? 8 : 0));
            }
            Console.WriteLine("Prototype: Tests: CPU: Register Access tests Passed");

            //computer.step();
            //Debug.Assert(computer.GetRegisters()[15] == 0x138 + 4);
            //new Thread(computer.run).Start();
            //computer.stopRun();
            //Thread.Sleep(250);
            //Debug.Assert(computer.GetRegisters()[15] == 0x138 + 8);
            //Console.WriteLine("Prototype: Tests: CPU: Running Program tests passed");

            //computer.Reset(arguments);
            //Debug.Assert(computer.GetMD5() == "3500a8bef72dfed358b25b61b7602cf1");
            //Debug.Assert(computer.GetStackPointer() == 0);
            //test4 = computer.GetRegisters();
            //for (int i = 0; i < 16; ++i)
            //{
            //    Debug.Assert(test3[i] == test4[i]);
            //}
            //computer.step();
            ////Debug.Assert(computer.GetRegisters()[15] == 0x138 + 4);
            //new Thread(computer.run).Start();
            //computer.stopRun();
            //Thread.Sleep(250);
            ////Debug.Assert(computer.GetRegisters()[15] == 0x138 + 8);
            //Console.WriteLine("Prototype: Tests: CPU: Test reruns after reset passed");

            Console.WriteLine("Prototype: Tests: CPU: All tests Passed");
            traceFile.Close();
        }
    }
}
