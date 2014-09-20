using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    class CPU
    {
        public Memory ram;
        public Registers registers;
        bool n, z, c, f;

        public CPU(Options arguments)
        {
            uint entryPoint = Loader.PreloadRAM(arguments, ref ram);
            registers = new Registers(entryPoint);
            n = z = c = f = false;
        }

        // fetch command based on program counter and increment program counter
        public uint fetch()
        {
            uint progC = registers.ReadRegister(15);
            registers.IncrementProgramCounter();
            return ram.ReadWord(progC);
        }

        // Decode command
        public uint /*???*/ decode()
        {
            return 0;
        }

        // Execute command
        public void execute()
        {
            System.Threading.Thread.Sleep(250);
        }

        // Convert flags to a string of 1s and 0s in the order of "nzcf"
        public string FlagsToString()
        {
            return (n ? "1" : "0") + (z ? "1" : "0") + (c ? "1" : "0") + (f ? "1" : "0"); 
        }

        // Get MD5 computation of ram
        public string GetMD5()
        {
            return ram.ComputeMD5();
        }

        // Get register number <register>
        public uint GetRegister(uint register)
        {
            return registers.ReadRegister(register);
        }

        // Read word from memory at <address>
        public uint GetWord(uint address)
        {
            if (address < ram.memorySize)
            {
                return ram.ReadWord(address);
            }
            return 0;
        }

        // Get stack pointer at register 13
        public uint GetStackPointer()
        {
            return registers.ReadRegister(13);
        }

        // Flag Getters
        public bool GetNFlag() { return n; }
        public bool GetZFlag() { return z; }
        public bool GetCFlag() { return c; }
        public bool GetFFlag() { return f; }

    }
}
