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
    class RAM
    {
        Byte[] memory;

        public RAM(uint memSize)
        {
            memory = new byte[memSize];
            Debug.WriteLine("Loader: RAM: Instantiating memory of " + memSize.ToString() + " bytes");
        }

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

        public uint ReadWord(uint address)
        {
            if ((address % 4) == 0)
            {
                return BitConverter.ToUInt32(memory, Convert.ToInt32(address));
            }
            return 0;
        }

        public void WriteHalfWord(uint address, uint data)
        {
            if ((address % 2) == 0)
            {
                memory[address] = Convert.ToByte(data & 255u);
                memory[address + 1] = Convert.ToByte((data >> 8) & 255u);
            }
        }

        public ushort ReadHalfWord(uint address)
        {
            if ((address % 2) == 0)
            {
                return BitConverter.ToUInt16(memory, Convert.ToInt32(address));
            }
            return 0;
        }

        public void WriteByte(uint address, byte data)
        {
            memory[address] = data;
        }

        public byte ReadByte(uint address)
        {
            return memory[address];
        }

        public bool TestFlag(uint address, int bit)
        {
            if (bit < 32 && bit >= 0)
            {
                return (ReadWord(address) & (1 << (bit - 1))) != 0;
            }
            return false;
        }

        public void SetFlag(uint address, int bit, bool flag)
        {
            if (bit < 32 && bit >= 0)
            {
                uint word = ReadWord(address);
                if (flag)
                {
                    WriteWord(address, word | (1u << (bit - 1)));
                }
            }
        }

        public void LoadRam(uint address, byte[] load)
        {
            MemoryStream stream = new MemoryStream(memory);
            stream.Write(load, Convert.ToInt32(address), load.Length);
        }

        public string ComputeMD5()
        {
            return MD5.Create().ComputeHash(memory).ToString();
        }
    }
}
