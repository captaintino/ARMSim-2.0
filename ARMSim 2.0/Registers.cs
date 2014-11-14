using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    public class Registers : Memory
    {
        public uint CPSR, SPSR_svc, SPSR_irq; // flags
        public uint LR_usr, LR_svc, LR_irq;
        public uint SP_usr, SP_svc, SP_irq;
        public Registers(uint EntryPoint)
            : base(64)
        {
            WriteRegister(15, EntryPoint);
            CPSR = Global.SUPERVISORMODE + 0xc0; // SYSTEM mode
            SP_usr = SP_svc = SP_irq = 0;
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

        public uint GetModeBits() { return Instruction.bits(CPSR, 4, 0); }
        public void SetModeBits(uint mode)
        {
            SetFlag(0, ((mode >> 0) & 1) == 1);
            SetFlag(1, ((mode >> 1) & 1) == 1);
            SetFlag(2, ((mode >> 2) & 1) == 1);
            SetFlag(3, ((mode >> 3) & 1) == 1);
            SetFlag(4, ((mode >> 4) & 1) == 1);
        }

        public uint GetCurrentSPSR()
        {
            switch (GetModeBits())
            {
                case Global.SUPERVISORMODE:
                    return SPSR_svc;
                case Global.IRQMODE:
                    return SPSR_irq;
                default:
                    return 0;
            }
        }
        public void UpdateCurrentSPSR(uint SPSR)
        {
            switch (GetModeBits())
            {
                case Global.SUPERVISORMODE:
                    SPSR_svc = SPSR;
                    break;
                case Global.IRQMODE:
                    SPSR_irq = SPSR;
                    break;
            }
        }
        public uint GetCPSR() { return CPSR; }
        public void SetCPSR(uint nCPSR) {
            if (GetModeBits() != Instruction.bits(nCPSR, 4, 0))
            {
                SwitchModes(Instruction.bits(nCPSR, 4, 0), CPSR & 0x1F);
            }
            else
            {
                CPSR = nCPSR;
            }
        }

        // Triggered by SWI/IRQ entry
        public void SaveCPSR(uint newMode)
        {
            switch (newMode)
            {
                case Global.SUPERVISORMODE:
                    SPSR_svc = CPSR;
                    break;
                case Global.IRQMODE:
                    SPSR_irq = CPSR;
                    break;
            }
        }

        //
        public void UpdateCPSR(uint newMode)
        {
            uint oldmode = CPSR;
            switch (Instruction.bits(CPSR, 4, 0))
            {
                case Global.SYSTEMMODE:
                    //oldmode = CPSR;
                    break;
                case Global.SUPERVISORMODE:
                    oldmode = SPSR_svc;
                    break;
                case Global.IRQMODE:
                    oldmode = SPSR_irq;
                    break;
            }
            switch (newMode)
            {
                case Global.SYSTEMMODE:
                    CPSR = oldmode;
                    break;
                case Global.SUPERVISORMODE:
                    //SPSR_irq = oldmode;
                    CPSR = oldmode; // ???
                    break;
            }
        }

        public uint getCorrectr15(){
            switch (GetCPSR() & 0x1F)
            {
                case Global.IRQMODE:
                    return LR_irq;
                case Global.SUPERVISORMODE:
                    return LR_svc + 4;
            }
            return 0;
        }

        public void SwitchModes(uint newMode, uint oldMode)
        {
            switch (newMode)
            {
                case Global.SUPERVISORMODE:
                    //SPSR_svc = CPSR;
                    switch (oldMode)
                    {
                        case Global.SYSTEMMODE:
                            // LR -> LR usr
                            LR_usr = ReadRegister(14);
                            // LR -> LR_svc
                            LR_svc = ReadRegister(15) - 4; // pointing EXACTLY at the right instruction
                            WriteRegister(14, LR_svc);
                            break;
                        case Global.IRQMODE:
                            //LR_irq = ReadRegister(14); // ????
                            WriteRegister(14, LR_svc); // ????
                            break;
                    }
                    SetModeBits(Global.SUPERVISORMODE);
                    break;
                case Global.IRQMODE:
                    //SPSR_irq = CPSR;
                    // LR -> LR_irq
                    LR_irq = ReadRegister(15); // pointing EXACTLY at the right instruction
                    switch (oldMode)
                    {
                        case Global.SYSTEMMODE:
                            // LR -> LR usr
                            LR_usr = ReadRegister(14);
                            break;
                        case Global.SUPERVISORMODE:
                            //LR_irq = ReadRegister(14);
                            break;
                    }
                    WriteRegister(14, LR_irq);
                    SetModeBits(Global.IRQMODE);
                    break;
                case Global.SYSTEMMODE:
                    switch (oldMode)
                    {
                        case Global.SUPERVISORMODE:
                            //CPSR = SPSR_svc;
                            WriteRegister(15, ReadRegister(14) + 4); // 4 > to handle fetch - 8
                            WriteRegister(14, LR_usr);
                            break;
                        case Global.IRQMODE:
                            //CPSR = SPSR_svc;
                            WriteRegister(15, LR_irq);
                            WriteRegister(14, LR_usr);
                            break;
                    }
                    SetModeBits(Global.SYSTEMMODE);
                    break;
            }
            // bank stack pointers:
            switch (oldMode)
            {
                case Global.SYSTEMMODE:
                    SP_usr = ReadRegister(13);
                    break;
                case Global.SUPERVISORMODE:
                    SP_svc = ReadRegister(13);
                    break;
                case Global.IRQMODE:
                    SP_irq = ReadRegister(13);
                    break;
            }
            switch (newMode)
            {
                case Global.SYSTEMMODE:
                    WriteRegister(13, SP_usr);
                    break;
                case Global.SUPERVISORMODE:
                    WriteRegister(13, SP_svc);
                    break;
                case Global.IRQMODE:
                    WriteRegister(13, SP_irq);
                    break;
            }
        }
    }
}
