using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ARMSim_2._0
{
    class Computer
    {
        CPU cpu;
        bool stop = true;
        FileStream trace;
        uint stepNumber;

        public Computer(Options arguments, ref FileStream tr)
        {
            cpu = new CPU(arguments);
            trace = tr;
            stepNumber = 0;
        }

        public void stopRun()
        {
            stop = true;
        }

        public void run()
        {
            stop = false;
            uint num;

            do
            {
                // rewrite
                uint progc = cpu.registers.ReadRegister(15);
                num = cpu.fetch(progc);
                num = 1;
                if (num == 0)
                {
                    break;
                    // trigger end of program
                }
                cpu.decode();
                cpu.execute();
                if (trace != null)
                    WriteLog(progc);
                stepNumber++;
            } while (!stop);
        }

        public void step()
        {
            // rewrite
            uint progc = cpu.registers.ReadRegister(15);
            uint num = cpu.fetch(progc);
            num = 1;
            if (num == 0)
            {
                return;
                // trigger end of program
            }
            cpu.decode();
            cpu.execute();
            if (trace != null)
                WriteLog(progc);
            stepNumber++;
        }

        private void WriteLog(uint programcounter)
        {
            List<uint> registers = GetRegisters();
            string write = stepNumber.ToString() + " " + programcounter.ToString() + " " + GetMD5() + " " + cpu.FlagsToString() + " " + String.Format("{0:X8} {0:X8} {0:X8} {0:X8}", registers[0], registers[1], registers[2], registers[3]) + 
            "\r\n" + String.Format("{0:X8} {0:X8} {0:X8} {0:X8}", registers[4], registers[5], registers[6], registers[7], registers[8], registers[9]) +
            "\r\n" + String.Format("{0:X8} {0:X8} {0:X8} {0:X8}", registers[10], registers[11], registers[12], registers[13], registers[14], registers[15]) + "\r\n";
            byte[] bytes = new byte[write.Length * sizeof(char)];
            System.Buffer.BlockCopy(write.ToCharArray(), 0, bytes, 0, bytes.Length);
            trace.Write(bytes, 0, bytes.Length);
        }

        public void Reset(Options arguments)
        {
            cpu = new CPU(arguments);
            stepNumber = 0;
        }

        public string GetMD5()
        {
            return cpu.ram.ComputeMD5();
        }

        public List<uint> GetRegisters()
        {
            List<uint> regs = new List<uint>();
            for (uint i = 0; i < 16; ++i)
            {
                regs.Add(cpu.registers.ReadRegister(i));
            }
            return regs;
        }

        public uint GetWord(uint address)
        {
            if (address < cpu.ram.memorySize)
            {
                return cpu.ram.ReadWord(address);
            }
            return 0;
        }

        public uint GetStackPointer()
        {
            return cpu.registers.ReadRegister(11);
        }

    }
}
