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
        Form1 parentForm = null;
        uint stepNumber;

        public Computer(Options arguments, ref FileStream tr)
        {
            cpu = Loader.PreloadCPU(arguments);
            trace = tr;
            stepNumber = 0;
        }

        // Set <parentForm>
        public void ProvideParentForm(Form1 parent) { parentForm = parent; }

        public bool Initialized() { return cpu.GetRegister(15) != 0; }

        public void stopRun()
        {
            stop = true;
        }

        // Perform Fetch, Decode, Execute process in loop
        public void run()
        {
            stop = false;
            do
            {
                FetchDecodeExecute();
            } while (!stop);
        }

        // Perform Fetch, Decode, Execute process once
        public void step()
        {
            FetchDecodeExecute();
        }

        // Perform Fetch, Decode, and Execute commands. Also triggers Delegate on parentform on program end
        private void FetchDecodeExecute()
        {
            stepNumber++;
            // rewrite
            uint progc = cpu.registers.ReadRegister(15);
            uint num = cpu.fetch();
            if (num == 0)
            {
                stop = true;
                // trigger end of program
                if (parentForm != null)
                    parentForm.Invoke(parentForm.myDelegate);
                return;
            }
            Instruction ins = cpu.decode(num);
            if (ins == null)
            {
                stop = true;
                // trigger end of program
                if (parentForm != null)
                    parentForm.Invoke(parentForm.myDelegate);
                return;
            }
            cpu.execute(ins);
            if (trace != null)
                WriteLog(progc);
        }

        // Write to tracefile
        private void WriteLog(uint programcounter)
        {
            List<uint> registers = GetRegisters();
            string write = stepNumber.ToString() + " " + programcounter.ToString() + " " + GetMD5() + " " + cpu.FlagsToString() + " " + String.Format("{0:X8} {0:X8} {0:X8} {0:X8}", registers[0], registers[1], registers[2], registers[3]) + 
            "\r\n" + String.Format("{0:X8} {0:X8} {0:X8} {0:X8}", registers[4], registers[5], registers[6], registers[7], registers[8], registers[9]) +
            "\r\n" + String.Format("{0:X8} {0:X8} {0:X8} {0:X8}", registers[10], registers[11], registers[12], registers[13], registers[14], registers[15]) + "\r\n";
            byte[] bytes = new byte[write.Length * sizeof(char)];
            System.Buffer.BlockCopy(write.ToCharArray(), 0, bytes, 0, bytes.Length);
            try
            {
                trace.Write(bytes, 0, bytes.Length);
            }
            catch { }
        }

        // Rebuild cpu with <arguments>
        public void Reset(Options arguments)
        {
            cpu = Loader.PreloadCPU(arguments);
            stepNumber = 0;
        }

        // Get MD5 computation of RAM
        public string GetMD5()
        {
            return cpu.GetMD5();
        }

        // return register values as uint list
        public List<uint> GetRegisters()
        {
            List<uint> regs = new List<uint>();
            for (uint i = 0; i < 16; ++i)
            {
                regs.Add(cpu.GetRegister(i));
            }
            return regs;
        }

        // Get word from memory at <address> if possible
        public uint GetWord(uint address)
        {
            return cpu.GetWord(address);
        }

        // Get stack pointer value from register.
        public uint GetStackPointer()
        {
            return cpu.GetStackPointer();
        }

        // return List of the flags in order of NZCF
        public List<bool> GetFlags()
        {
            List<bool> flags = new List<bool>();
            flags.Add(cpu.GetNFlag());
            flags.Add(cpu.GetZFlag());
            flags.Add(cpu.GetCFlag());
            flags.Add(cpu.GetFFlag());
            return flags;
        }

    }
}
