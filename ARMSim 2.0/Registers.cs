using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    public class Registers : Memory
    {
        public uint CPSR, SPSR_svc; // flags
        public uint LR_usr, LR_svc, SP_svc;
        public Registers(uint EntryPoint) : base(64)
        {
            WriteRegister(15, EntryPoint);
            CPSR = 31; // SYSTEM mode
        }

        // Reads register number <registerNumber> (zero-based)
        public uint ReadRegister(uint registerNumber)
        {
            if (registerNumber < 16)
            {
                return ReadWord(registerNumber * 4);
            }
            else
            {
                return 0;
            }
        }

        // Writes <data> to register number <registerNumber> (zero-based)
        public void WriteRegister(uint registerNumber, uint data)
        {
            if (registerNumber < 16)
            {
                WriteWord(registerNumber * 4, data);
            }
        }

        // add 4 to r15
        public void IncrementProgramCounter()
        {
            WriteRegister(15, ReadRegister(15) + 4);
        }

        // Check if <bit> is set in the CPSR
        public bool TestFlag(int bit)
        {
            if (bit < 32 && bit >= 0)
            {
                return (CPSR & (1 << bit)) != 0;
            }
            return false;
        }

        // Set <bit> in CPSR to <flag>
        public void SetFlag(int bit, bool flag)
        {
            if (bit < 32 && bit >= 0)
            {
                if (flag)
                {
                    CPSR |= (1u << bit);
                }
                else
                {
                    CPSR &= (~(1u << bit));
                }
            }
        }

        public void SwitchModes(uint newMode)
        {
            SPSR_svc = CPSR;
            if (newMode == Global.SUPERVISORMODE)
            {
                LR_svc = ReadRegister(14);
                // SWAP BANKED REGISTERS
            }
        }

        public void SwitchModesBack()
        {
            CPSR = SPSR_svc;
            
        }
    }
}
