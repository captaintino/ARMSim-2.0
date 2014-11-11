using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ARMSim_2._0
{
    public class Computer
    {
        CPU cpu;
        bool stop = true;
        public bool IRQ { get; set; }
        FileStream trace;
        public Form1 parentForm = null;
        uint stepNumber;
        public Queue<char> inputBuffer;
        public Queue<char> outputBuffer;

        public Computer(Options arguments, ref FileStream tr)
        {
            cpu = Loader.PreloadCPU(arguments, this);
            trace = tr;
            stepNumber = 0;
            IRQ = false;
            inputBuffer = new Queue<char>();
            outputBuffer = new Queue<char>();
        }

        // Set <parentForm>
        public void ProvideParentForm(Form1 parent) { parentForm = parent; }

        // Determine if executable has been loaded
        public bool Initialized() { return cpu.GetRegister(15) != 0; }

        // End the looping of run()
        public void stopRun()
        {
            stop = true;
            // trigger end of program
        }

        // Perform Fetch, Decode, Execute process in loop
        public void run()
        {
            if (cpu.fetch() == 0 || cpu.fetch() == 0xEF000000 /* SWI #0 */)
            {
                if (parentForm != null)
                    parentForm.Invoke(parentForm.myDelegate);
                return;
            }
            stop = false;
            do
            {
                FetchDecodeExecute();
            } while (!stop);
            if (parentForm != null)
                parentForm.Invoke(parentForm.myDelegate);
        }

        // Perform Fetch, Decode, Execute process once
        public void step()
        {
            if (cpu.fetch() != 0 && cpu.fetch() != 0xEF000000 /* SWI #0 */)
            {
                stop = false;
                FetchDecodeExecute();
            }
            stop = true;
        }

        // Perform Fetch, Decode, and Execute commands. Also triggers Delegate on parentform on program end
        private void FetchDecodeExecute()
        {
            // rewrite
            uint progc = cpu.registers.ReadRegister(15);
            uint num = cpu.fetch();
            if (num == 0)
            {
                endProgram(progc);
                return;
            }
            Instruction ins = cpu.decode(num, progc - 8);
            if (ins == null)
            {
                endProgram(progc);
                return;
            }
            cpu.execute(ins);
            stepNumber++;
            if (trace != null)
                WriteLog(progc);
            if(!stop)
                cpu.registers.IncrementProgramCounter();
            if (IRQ && !cpu.GetIFlag())
            {
                IRQ = false;
                cpu.registers.SwitchModes(Global.IRQMODE);
                cpu.SetIFlag(true);
                cpu.registers.WriteRegister(15, 0x18 + 8); // no increment
            }
        }

        // trigger end of program and que GUI update if possible
        private void endProgram(uint progc)
        {
            stop = true;
            stepNumber++;
            // trigger end of program
            if (parentForm != null)
                parentForm.Invoke(parentForm.myDelegate);
            if (trace != null)
                WriteLog(progc);
            cpu.registers.IncrementProgramCounter();
        }

        // Write to tracefile
        private void WriteLog(uint programcounter)
        {
            List<uint> registers = GetRegisters();
            string write = String.Format("{0:d6} {1:X8} ", stepNumber, programcounter - 8) + GetMD5() + " " + cpu.FlagsToString() + "   " + String.Format("0={0:X8}  1={1:X8}  2={2:X8}  3={3:X8}\r\n        ", registers[0], registers[1], registers[2], registers[3]) +
            String.Format("4={0:X8}  5={1:X8}  6={2:X8}  7={3:X8}  8={4:X8}  9={5:X8}\r\n       ", registers[4], registers[5], registers[6], registers[7], registers[8], registers[9]) +
            String.Format("10={0:X8} 11={1:X8} 12={2:X8} 13={3:X8} 14={4:X8}\r\n", registers[10], registers[11], registers[12], registers[13], registers[14]);
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
            cpu = Loader.PreloadCPU(arguments, this);
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

        // Get specific register values
        public uint GetStackPointer() { return cpu.GetStackPointer(); }
        public uint getProgramCounter() { return cpu.getProgramCounter(); }
        public uint GetProcMode() { return cpu.GetModeBits(); }

        // return List of the flags in order of NZCF
        public List<bool> GetFlags()
        {
            List<bool> flags = new List<bool>();
            flags.Add(cpu.GetNFlag());
            flags.Add(cpu.GetZFlag());
            flags.Add(cpu.GetCFlag());
            flags.Add(cpu.GetVFlag());
            return flags;
        }

        // Gets a particular instruction from memory at <addr>
        public Instruction getInstruction(uint addr)
        {
            return cpu.decode(cpu.fetchParticular(addr), addr);
        }

    }
}
