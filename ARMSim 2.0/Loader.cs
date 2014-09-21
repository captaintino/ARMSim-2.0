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
    class Loader
    {
        // Extract pertinent data from file requested in <arguments>, load into <ram>, and return the program entrypoint.
        public static CPU PreloadCPU(Options arguments)
        {
            Memory ram = new Memory(arguments.memorySize);
            if (arguments.fileName != "")
            {
                try
                {
                    string elfFilename = arguments.fileName;
                    ELF elfHeader;

                    Debug.WriteLine("Loader: Opening " + elfFilename);
                    using (FileStream strm = new FileStream(elfFilename, FileMode.Open))
                    {
                        elfHeader = ExtractELFHeader(strm);

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

                        Console.WriteLine("Loader: Compute MD5: " + ram.ComputeMD5());

                    }
                    Registers regs = new Registers(elfHeader.e_entry);
                    return new CPU(ram, regs);
                }
                catch
                {
                    Console.WriteLine("Loader: ERROR OCCURRED DURING RAM LOADING");
                    //Program.QuitProgram();
                    return new CPU(new Memory(arguments.memorySize), new Registers(0));
                }
            }
            else
            {
                return new CPU(new Memory(arguments.memorySize), new Registers(0));
            }
        }

        // Extract ELF Header from <strm>
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

        // Extract segment headers from <strm> using information in <elfheader>
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

        // Convert byte array to struct
        static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(),
                typeof(T));
            handle.Free();
            return stuff;
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
    public struct SegmentHeader
    {
        public uint p_type;
        public uint p_offset;
        public uint p_vaddr;
        public uint p_paddr;
        public uint p_filesz;
        public uint p_memsz;
        public uint p_flags;
        public uint p_align;
    }
}
