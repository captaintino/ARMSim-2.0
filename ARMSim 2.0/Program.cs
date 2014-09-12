using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace ARMSim_2._0
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                QuitProgram();
            }

            Options arguments = new Options(args);

            Debug.WriteLine("Loader: Options: filename: " + arguments.fileName);
            Debug.WriteLine("Loader: Options: memSize: " + arguments.memorySize.ToString());
            Debug.WriteLine("Loader: Options: testMode: " + (arguments.testMode ? "True" : "False"));

            if (arguments.fileName == "" && !arguments.testMode)
            {
                QuitProgram();
            }

            if (!arguments.testMode)
            {
                RAM ram = Loader.PreloadRAM(arguments);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else
            {
                TestApp();
            }
        }

        // Write Usage Statement and end program
        public static void QuitProgram()
        {
            Console.WriteLine("Usage: armsim [ --load elf-file ] [ --mem memory-size ] [ --test]");
            Environment.Exit(1);
            
        }

        // Run All Unit Test Classes
        public static void TestApp()
        {
            TestOptions to = new TestOptions();
            to.RunTests();
            TestRAM tr = new TestRAM();
            tr.RunTests();
            TestLoader tl = new TestLoader();
            tl.RunTests();
        }
    }
}
