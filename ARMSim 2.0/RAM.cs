using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;

namespace ARMSim_2._0
{
    class Memory
    {
        private Byte[] memory;
        public uint memorySize;

        public Memory(uint memSize = 32768u)
        {
            memory = new byte[memSize];
            memorySize = memSize;
            Debug.WriteLine("Loader: Memory: Instantiating memory of " + memSize.ToString() + " bytes");
        }

        // Write 32bits of <data> to <address> 
        public void WriteWord(uint address, uint data)
        {
            if ((address % 4) == 0)
            {
                memory[address] = Convert.ToByte(data & 255u);
                memory[address + 1] = Convert.ToByte((data >> 8) & 255u);
                memory[address + 2] = Convert.ToByte((data >> 16) & 255u);
                memory[address + 3] = Convert.ToByte((data >> 24) & 255u);
            }
        }

        // Read 32bits of data from <address> 
        public uint ReadWord(uint address)
        {
            if ((address % 4) == 0)
            {
                return BitConverter.ToUInt32(memory, Convert.ToInt32(address));
            }
            return 0;
        }

        // Write 16bits of <data> to <address> 
        public void WriteHalfWord(uint address, uint data)
        {
            if ((address % 2) == 0)
            {
                memory[address] = Convert.ToByte(data & 255u);
                memory[address + 1] = Convert.ToByte((data >> 8) & 255u);
            }
        }

        // Read 16bits of data from <address> 
        public ushort ReadHalfWord(uint address)
        {
            if ((address % 2) == 0)
            {
                return BitConverter.ToUInt16(memory, Convert.ToInt32(address));
            }
            return 0;
        }

        // Write 8bits of <data> to <address> 
        public void WriteByte(uint address, byte data)
        {
            memory[address] = data;
        }

        // Read 8bits of data from <address> 
        public byte ReadByte(uint address)
        {
            return memory[address];
        }

        // Check if <bit> of <address> is set
        public bool TestFlag(uint address, int bit)
        {
            if (bit < 32 && bit >= 0)
            {
                return (ReadWord(address) & (1 << bit)) != 0;
            }
            return false;
        }

        // Set <bit> of <address> to <flag>
        public void SetFlag(uint address, int bit, bool flag)
        {
            if (bit < 32 && bit >= 0)
            {
                uint word = ReadWord(address);
                if (flag)
                {
                    WriteWord(address, word | (1u << bit));
                }
                else
                {
                    WriteWord(address, word & (~ (1u << bit)));
                }
            }
        }

        // Load chunk of memory contained in <load> into simulated memory at <address>
        public void LoadRam(uint address, byte[] load)
        {
            MemoryStream stream = new MemoryStream(memory);
            stream.Seek((long)address, SeekOrigin.Begin);
            stream.Write(load, 0, load.Length);
        }

        // Computer MD5 value of simulated RAM
        public string ComputeMD5()
        {
            StringBuilder sBuilder = new StringBuilder();
            byte[] md5 = MD5.Create().ComputeHash(memory);
            for (int i = 0; i < md5.Length; i++)
            {
                sBuilder.Append(md5[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
