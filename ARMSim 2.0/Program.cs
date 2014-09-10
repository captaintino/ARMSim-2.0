using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

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

            if (arguments.fileName == "")
            {
                QuitProgram();
            }

            if (!arguments.testMode)
            {
                RAM ram = new RAM(arguments.memorySize);
                string elfFilename = arguments.fileName;

                using (FileStream strm = new FileStream(elfFilename, FileMode.Open))
                {
                    ELF elfHeader = new ELF();
                    byte[] data = new byte[Marshal.SizeOf(elfHeader)];

                    // Read ELF header data
                    strm.Read(data, 0, data.Length);
                    // Convert to struct
                    elfHeader = ByteArrayToStructure<ELF>(data);

                    Debug.WriteLine("Loader: FileRead: Entry point: " + elfHeader.e_entry.ToString("X4"));
                    Debug.WriteLine("Loader: FileRead: Number of program header entries: " + elfHeader.e_phnum);

                    // Read first program header entry
                    strm.Seek(elfHeader.e_phoff, SeekOrigin.Begin);
                    data = new byte[elfHeader.e_phentsize];
                    strm.Read(data, 0, (int)elfHeader.e_phentsize);

                    // Now, do something with it ... see cppreadelf for a hint

                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else
            {
                TestStuff();
            }
        }

        static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(),
                typeof(T));
            handle.Free();
            return stuff;
        }

        public static void readSegment(RAM ram, uint addressFile, uint addressRam, uint size)
        {

        }

        public static void QuitProgram()
        {
            Console.WriteLine("Usage: armsim [ --load elf-file ] [ --mem memory-size ] [ --test]");
            Environment.Exit(1);
            
        }

        public static void TestStuff()
        {
            //CALL TEST CODE ON ALL MODULES
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ELF
    {
        public byte EI_MAG0, EI_MAG1, EI_MAG2, EI_MAG3, EI_CLASS, EI_DATA, EI_VERSION;
        byte unused1, unused2, unused3, unused4, unused5, unused6, unused7, unused8, unused9;
        public ushort e_type;
        public ushort e_machine;
        public uint e_version;
        public uint e_entry;
        public uint e_phoff;
        public uint e_shoff;
        public uint e_flags;
        public ushort e_ehsize;
        public ushort e_phentsize;
        public ushort e_phnum;
        public ushort e_shentsize;
        public ushort e_shnum;
        public ushort e_shstrndx;
    }
}
