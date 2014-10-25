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

        public CPU(Memory newRam, Registers newRegisters)
        {
            ram = newRam;
            registers = newRegisters;
            n = z = c = f = false;
        }

        // Flag Getters
        public bool GetNFlag() { return n; }
        public bool GetZFlag() { return z; }
        public bool GetCFlag() { return c; }
        public bool GetFFlag() { return f; }

        // fetch command based on program counter and increment program counter
        public uint fetch() { return ram.ReadWord(registers.ReadRegister(15) - 8); }

        public uint fetchParticular(uint addr) { return ram.ReadWord(addr); }

        // Decode command
        public Instruction /*???*/ decode(uint data)
        {
            return Instruction.InstructionFactory(data);
        }

        // Execute command
        public void execute(Instruction instruction)
        {
            switch (instruction.Cond)
            {
                case 0:
                    if (false) return; // Z
                    else break;
                case 1:
                    if (false) return; // !Z
                    else break;
                case 2:
                    if (false) return; // C
                    else break;
                case 3:
                    if (false) return; // !C
                    else break;
                case 4:
                    if (false) return; // N
                    else break;
                case 5:
                    if (false) return; // !N
                    else break;
                case 6:
                    if (false) return; // V
                    else break;
                case 7:
                    if (false) return; // !V
                    else break;
                case 8:
                    if (false) return; // C && !Z
                    else break;
                case 9:
                    if (false) return; // !C || Z
                    else break;
                case 10:
                    if (false) return; // N == V
                    else break;
                case 11:
                    if (false) return; // N != V
                    else break;
                case 12:
                    if (false) return; // Z == 0 && N == V
                    else break;
                case 13:
                    if (false) return; // Z == 1 || N != V
                    else break;
            }
            instruction.execute(registers, ram);
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
        public uint GetStackPointer() { return registers.ReadRegister(13); }
        public uint getProgramCounter() { return registers.ReadRegister(15); }
    }
}
