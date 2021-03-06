﻿using System;
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
    public static class Global
    {
        //Modes
        public const uint USERMODE = 16;
        public const uint FIQMODE = 17;
        public const uint IRQMODE = 18;
        public const uint SUPERVISORMODE = 19;
        public const uint ABORTMODE = 23;
        public const uint UNDEFINEDMODE = 27;
        public const uint SYSTEMMODE = 31;
        //Masks
        public const uint UNALLOCMASK = 0x0FFFFF00u;
        public const uint USERMASK = 0xF0000000u;
        public const uint PRIVMASK = 0x0000000Fu;
        public const uint STATEMASK = 0x00000020u;
        // Keyboard conversions
        public static readonly char[] convertNumbers = { ')', '!', '@', '#', '$', '%', '^', '&', '*', '(' };
        public static readonly Dictionary<int, Tuple<char, char>> convertPunctuation = new Dictionary<int, Tuple<char, char>>() {
        { 0xba, new Tuple<char,char>(';',':')},
        { 0xbb, new Tuple<char,char>('=','+')},
        { 0xbc, new Tuple<char,char>(',','<')},
        { 0xbd, new Tuple<char,char>('-','_')},
        { 0xbe, new Tuple<char,char>('.','>')},
        { 0xbf, new Tuple<char,char>('/','?')},
        { 0xc0, new Tuple<char,char>('`','~')},
        { 0xdb, new Tuple<char,char>('[','{')},
        { 0xdc, new Tuple<char,char>('\\','|')},
        { 0xdd, new Tuple<char,char>(']','}')},
        { 0xde, new Tuple<char,char>('\'','"')}
        };    
    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Options arguments = new Options(new string[0]);
            if (args.Length > 0)
            {
                arguments = new Options(args);

                Debug.WriteLine("Loader: Options: filename: " + arguments.fileName);
                Debug.WriteLine("Loader: Options: memSize: " + arguments.memorySize.ToString());
                Debug.WriteLine("Loader: Options: testMode: " + (arguments.testMode ? "True" : "False"));
                Debug.WriteLine("Loader: Options: execMode: " + (arguments.testMode && arguments.fileName != "" ? "True" : "False"));

                if (arguments.fileName == "" && !arguments.testMode && !arguments.exec)
                {
                    QuitProgram();
                }
                else if(arguments.testMode)
                {
                    TestApp();
                    Environment.Exit(0); // All Tests Passed    
                }
                else if (arguments.fileName != "" && arguments.exec)
                {
                    Form1 simulator = new Form1(arguments);
                    simulator.runOnce();
                    Environment.Exit(1);
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(arguments));
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
            TestMemory tm = new TestMemory();
            tm.RunTests();
            TestRegisters tr = new TestRegisters();
            tr.RunTests();
            TestCPU tc = new TestCPU();
            tc.RunTests();
            TestLoader tl = new TestLoader();
            tl.RunTests();
            TestComputer tco = new TestComputer();
            tco.RunTests();
            TestInstruction ti = new TestInstruction();
            ti.RunTests();
        }
    }
}
