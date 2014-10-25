using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    public class LSInstruction : Instruction
    {
        protected bool p, u, w, l;
        bool b;
        ushort immediate;
        Operand2 op2;

        protected LSInstruction() { }
        public LSInstruction(uint data)
        {
            this.data = data;
            this.decode();
        }

        // parse command into the proper variables
        public override void decode()
        {
            Cond = bitsb(data, 31, 28);
            i = bitsb(data, 25, 25) == 1;
            p = bitsb(data, 24, 24) == 1;
            u = bitsb(data, 23, 23) == 1;
            b = bitsb(data, 22, 22) == 1;
            w = bitsb(data, 21, 21) == 1;
            l = bitsb(data, 20, 20) == 1;
            Rn = bitss(data, 19, 16);
            Rd = bitss(data, 15, 12);
            if (i)
            {
                op2 = new Operand2(data, !i); // !i because load/store reverses the meaning of the bit
            }
            else
            {
                immediate = bitss(data, 11, 0);
            }
        }

        // perform command on <regs> and <ram>
        public override void execute(Registers regs, Memory ram)
        {
            RAMReference = ram;
            registersReference = regs;
            // SAFETY?
            int offset = (int)((i ? op2.execute(registersReference) : immediate) * (u ? 1 : -1)); // positive/negative
            // SAFETY?
            uint address = registersReference.ReadRegister(Rn);
            if (p) // Pre
            {
                address = (uint)(address + offset);
            }
            if (l) // LOAD
            {
                if (b) // byte
                {
                    registersReference.WriteRegister(Rd, RAMReference.ReadByte(address));
                }
                else // word
                {
                    registersReference.WriteRegister(Rd, RAMReference.ReadWord(address));
                }
            }
            else // STORE
            {
                if (b) // byte
                {
                    RAMReference.WriteByte(address, (byte)(registersReference.ReadRegister(Rd) % 256));
                }
                else // word
                {
                    RAMReference.WriteWord(address, registersReference.ReadRegister(Rd));
                }
            }

            if (p && w) // writeback
            {
                registersReference.WriteRegister(Rn, address);
            }
            else if (!p)
            {
                registersReference.WriteRegister(Rn, (uint)(address + offset)); // post indexed writeback
            }
        }

        // Convert command to assembly string 
        public override string ToString()
        {
            string str = (l ? "ldr" : "str");
            if (b) str += "b";
            str += " r" + Rd;
            str += ", [r" + Rn;
            string offset = "";
            if (i)
            {
                offset = op2.ToString();
            }
            else if (immediate > 0)
            {
                offset = immediate.ToString();
            }
            if (offset != "")
            {                                   // SUPPORT FOR POSITIVE/NEGATIVE UNKNOWN
                if (p)
                {
                    str += ", " + (i ? op2.ToString() : "#" + immediate.ToString()) + "]";
                }
                else
                {
                    str += "], " + (i ? op2.ToString() : "#" + immediate);
                }
            }
            else
            {
                str += "]";
            }
            if (w && p) str += " !";
            return str;
        }
    }
}
