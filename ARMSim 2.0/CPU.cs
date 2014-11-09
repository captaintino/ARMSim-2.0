using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    public class CPU
    {
        public Memory ram;
        public Registers registers;
        private Computer computer;

        public CPU(Memory newRam, Registers newRegisters, Computer comp)
        {
            ram = newRam;
            registers = newRegisters;
            computer = comp;
        }

        // Flag Getters
        public bool GetNFlag() { return registers.TestFlag(31); }
        public bool GetZFlag() { return registers.TestFlag(30); }
        public bool GetCFlag() { return registers.TestFlag(29); }
        public bool GetVFlag() { return registers.TestFlag(28); }
        public bool GetIFlag() { return registers.TestFlag(7); }
        public bool GetFFlag() { return registers.TestFlag(6); }
        public uint GetModeBits() { return Instruction.bits(registers.CPSR, 4, 0); }
        public void SetNFlag(bool setVal) { registers.SetFlag(31, setVal); }
        public void SetZFlag(bool setVal) { registers.SetFlag(30, setVal); }
        public void SetCFlag(bool setVal) { registers.SetFlag(29, setVal); }
        public void SetVFlag(bool setVal) { registers.SetFlag(28, setVal); }
        public void SetIFlag(bool setVal) { registers.SetFlag(7, setVal); }
        public void SetFFlag(bool setVal) { registers.SetFlag(6, setVal); }
        public void SetModeBits(uint mode)
        {
            registers.SetFlag(0, ((mode >> 0) & 1) == 1);
            registers.SetFlag(1, ((mode >> 1) & 1) == 1);
            registers.SetFlag(2, ((mode >> 2) & 1) == 1);
            registers.SetFlag(3, ((mode >> 3) & 1) == 1);
            registers.SetFlag(4, ((mode >> 4) & 1) == 1);
        }

        // fetch command based on program counter and increment program counter
        public uint fetch() { return ram.ReadWord(registers.ReadRegister(15) - 8); }

        public uint fetchParticular(uint addr) { return ram.ReadWord(addr); }

        // Decode command
        public Instruction /*???*/ decode(uint data, uint progc = 0)
        {
            return Instruction.InstructionFactory(data, progc);
        }

        // Execute command
        public void execute(Instruction instruction)
        {
            switch (instruction.Cond)
            {
                case 0:
                    if (GetZFlag()) break; // Z
                    else return;
                case 1:
                    if (!GetZFlag()) break; // !Z
                    else return;
                case 2:
                    if (GetCFlag()) break; // C
                    else return;
                case 3:
                    if (!GetCFlag()) break; // !C
                    else return;
                case 4:
                    if (GetNFlag()) break; // N
                    else return;
                case 5:
                    if (!GetNFlag()) break; // !N
                    else return;
                case 6:
                    if (GetVFlag()) break; // V
                    else return;
                case 7:
                    if (!GetVFlag()) break; // !V
                    else return;
                case 8:
                    if (GetCFlag() && !GetZFlag()) break; // C && !Z
                    else return;
                case 9:
                    if (!GetCFlag() || GetZFlag()) break; // !C || Z
                    else return;
                case 10:
                    if (GetNFlag() == GetVFlag()) break; // N == V
                    else return;
                case 11:
                    if (GetNFlag() != GetVFlag()) break; // N != V
                    else return;
                case 12:
                    if (GetZFlag() == false && GetNFlag() == GetVFlag()) break; // Z == 0 && N == V
                    else return;
                case 13:
                    if (GetZFlag() == true || GetNFlag() != GetVFlag()) break; // Z == 1 || N != V
                    else return;
            }
            instruction.execute(this);
        }

        // Convert flags to a string of 1s and 0s in the order of "nzcf"
        public string FlagsToString()
        {
            return (GetNFlag() ? "1" : "0") + (GetZFlag() ? "1" : "0") + (GetCFlag() ? "1" : "0") + (GetVFlag() ? "1" : "0"); 
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

        // Read/Write characters from the console
        public uint readChar()
        {
            if (computer.inputBuffer.Count > 0)
            {
                return computer.inputBuffer.Dequeue();
            }
            return 0;
        }
        public void writeChar(uint character)
        {
            if (character % 256 < 128)
            {
                computer.outputBuffer.Enqueue((char)character);
                computer.parentForm.Invoke(computer.parentForm.myDelegate2);
            }
        }
        // Get stack pointer at register 13
        public uint GetStackPointer() { return registers.ReadRegister(13); }
        public uint getProgramCounter() { return registers.ReadRegister(15); }

        public void StopComputer() { computer.stopRun(); }
    }
}
