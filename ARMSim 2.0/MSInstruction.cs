using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    class MSInstruction : Instruction
    {
        byte Rm;
        Operand2 op2;
        bool immediateMode, statusToReg, R, c, x, s, f;

        public MSInstruction(uint data)
        {
            this.data = data;
            this.decode();
        }

        // parse command into the proper variables
        public override void decode()
        {
            Cond = bitsb(data, 31, 28);
            immediateMode = bitsb(data, 25, 25) == 1;
            statusToReg = bitsb(data, 21, 21) == 0;
            R = bitsb(data, 22, 22) == 1;
            c = bitsb(data, 16, 16) == 1;
            x = bitsb(data, 17, 17) == 1;
            s = bitsb(data, 18, 18) == 1;
            f = bitsb(data, 19, 19) == 1;
            Rd = bitsb(data, 15, 12);
            Rm = bitsb(data, 3, 0);
            op2 = immediateMode ? new Operand2(data, true) : null;
        }

        // perform command on <regs> and <ram>
        public override void execute(CPU cpu)
        {
            registersReference = cpu.registers;
            if (statusToReg)
            {
                registersReference.WriteRegister(Rd, (R ? registersReference.GetCurrentSPSR() : registersReference.GetCPSR()));
            }
            else
            {
                uint byteMask = (f ? 0xFF000000u : 0) | (s ? 0xFF0000u : 0) | (x ? 0xFF00u : 0) | (c ? 0xFFu : 0);
                uint operand = immediateMode ? op2.execute(registersReference) : registersReference.ReadRegister(Rm);
                uint mask;
                if (R)
                {
                    mask = byteMask & (Global.USERMASK | Global.PRIVMASK | Global.STATEMASK);
                    registersReference.UpdateCurrentSPSR((registersReference.GetCurrentSPSR() & (~mask)) | (operand & mask));
                }
                else
                {
                    if (registersReference.GetModeBits() != Global.SYSTEMMODE) // In a priviledged mode
                    {
                        mask = byteMask & (Global.USERMASK | Global.PRIVMASK);
                    }
                    else
                    {
                        mask = byteMask & Global.USERMASK;
                    }
                    registersReference.SetCPSR((registersReference.GetCPSR() & (~mask)) | (operand & mask));
                }
            }
        }

        // Convert command to assembly string 
        public override string ToString()
        {
            if (statusToReg)
            {
                return "mrs" + conditional() + " r" + Rd + (R ? ", SPSR" : ", CPSR");
            }
            else
            {
                return "msr" + conditional() + (R ? " SPSR_" : " CPSR_") + (f ? "f" : "") + (s ? "s" : "") + (x ? "x" : "") + (c ? "c" : "")
                    + ", " + (immediateMode ? op2.ToString() : (" r" + Rm));
            }
        }
    }
}
