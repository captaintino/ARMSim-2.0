using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    class LSMulInstruction : LSInstruction
    {
        bool s;

        public LSMulInstruction(uint data)
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
            s = bitsb(data, 22, 22) == 1;
            w = bitsb(data, 21, 21) == 1;
            l = bitsb(data, 20, 20) == 1;
            Rn = bitss(data, 19, 16);
        }

        // perform command on <regs> and <ram>
        public override void execute(Registers regs, Memory ram)
        {
            RAMReference = ram;
            registersReference = regs;
            int address = (int)registersReference.ReadRegister(Rn);
            if (l) // LOAD
            {
                for (int i = 15; i >= 0; --i)
                {
                    if (bitsb(data, i, i) == 1)
                    {
                        if(p) address = (address + (u ? 4 : -4));
                        registersReference.WriteRegister((uint)i, RAMReference.ReadWord((uint)address));
                        if (!p) address = (address + (u ? 4 : -4));
                    }
                }
            }
            else // STORE
            {
                for (int i = 15; i >= 0; --i)
                {
                    if (bitsb(data, i, i) == 1)
                    {
                        if (p) address = (address + (u ? 4 : -4));
                        RAMReference.WriteWord((uint)address, registersReference.ReadRegister((uint)i));
                        if (!p) address = (address + (u ? 4 : -4));
                    }
                }
            }
            if (w)
            {
                registersReference.WriteRegister(Rn, (uint)(address - 4));
            }
        }

        // Convert command to assembly string 
        public override string ToString()
        {
            string str = l ? "ldm" : "stm";
            if (l)
            {
                str += (p ? "e" : "f") + (u ? "d" : "a");
            }
            else
            {
                str += (p ? "f" : "e") + (u ? "a" : "d");
            }
            str += " r" + Rn + (w ? "!" : "") + ", {r";
            for (int i = 15; i >= 0; --i)
            {
                if (bitsb(data, i, i) == 1)
                {
                    str += i + ", r";
                }
            }
            return str.Remove(str.Length - 3) + "}";
        }
    }
}
