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
                RAM ram = PreloadRAM(arguments);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else
            {
                TestStuff();
            }
        }

        public static RAM PreloadRAM(Options arguments)
        {
            try
            {
                RAM ram = new RAM(arguments.memorySize);
                string elfFilename = arguments.fileName;

                Debug.WriteLine("Loader: Opening " + elfFilename);
                using (FileStream strm = new FileStream(elfFilename, FileMode.Open))
                {
                    ELF elfHeader = ExtractELFHeader(strm);

                    // Read first program header entry
                    List<SegmentHeader> segmentHeaders = ExtractSegmentHeader(strm, elfHeader);
                    byte[] data;
                    foreach (SegmentHeader seg in segmentHeaders)
                    {
                        Debug.WriteLine("Loader: Segment: Address = " + seg.p_paddr.ToString() + " Offset = " + seg.p_offset.ToString() + " Size = " + seg.p_memsz.ToString());
                        strm.Seek(seg.p_offset, SeekOrigin.Begin);
                        data = new byte[seg.p_filesz];
                        strm.Read(data, 0, (int)seg.p_filesz);
                        ram.LoadRam(seg.p_vaddr, data);
                    }

                    StringBuilder sBuilder = new StringBuilder();
                    byte[] md5 = ram.ComputeMD5();
                    for (int i = 0; i < md5.Length; i++)
                    {
                        sBuilder.Append(md5[i].ToString("x2"));
                    }

                    Debug.WriteLine("Loader: Compute MD5: " + sBuilder.ToString());

                }
                return ram;
            }
            catch
            {
                Console.WriteLine("Loader: ERROR OCCURRED DURING RAM LOADING");
                QuitProgram();
                return null; // Not all code paths returned a value
            }
        }

        public static ELF ExtractELFHeader(FileStream strm)
        {
            ELF elfHeader = new ELF();
            byte[] data = new byte[Marshal.SizeOf(elfHeader)];

            // Read ELF header data
            strm.Read(data, 0, data.Length);
            // Convert to struct
            elfHeader = ByteArrayToStructure<ELF>(data);

            Debug.WriteLine("Loader: FileRead: Entry point: " + elfHeader.e_entry.ToString("X4"));
            Debug.WriteLine("Loader: FileRead: Number of program header entries: " + elfHeader.e_phnum);

            return elfHeader;
        }

        public static List<SegmentHeader> ExtractSegmentHeader(FileStream strm, ELF elfHeader)
        {
            List<SegmentHeader> segmentHeaders = new List<SegmentHeader>();
            byte[] data;
            strm.Seek(elfHeader.e_phoff, SeekOrigin.Begin);
            for (int i = 0; i < elfHeader.e_phnum; ++i)
            {
                data = new byte[elfHeader.e_phentsize];
                strm.Read(data, 0, (int)elfHeader.e_phentsize);
                SegmentHeader seg = ByteArrayToStructure<SegmentHeader>(data);
                segmentHeaders.Add(seg);
            }
            Debug.WriteLine("Loader: FileRead: Program Header Quantity: " + segmentHeaders.Count);
            return segmentHeaders;
        }

        static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(),
                typeof(T));
            handle.Free();
            return stuff;
        }

        public static void QuitProgram()
        {
            Console.WriteLine("Usage: armsim [ --load elf-file ] [ --mem memory-size ] [ --test]");
            Environment.Exit(1);
            
        }

        public static void TestStuff()
        {
            TestOptions to = new TestOptions();
            TestRAM tr = new TestRAM();
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

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SegmentHeader{
	    public uint	p_type;
	    public uint	p_offset;
	    public uint	p_vaddr;
	    public uint	p_paddr;
	    public uint	p_filesz;
	    public uint	p_memsz;
	    public uint	p_flags;
	    public uint	p_align;
    }
}
