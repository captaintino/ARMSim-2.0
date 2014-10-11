using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    class BrInstruction : Instruction
    {
        bool l;
        uint immediate;

        public BrInstruction(uint data)
        {
            this.data = data;
            this.decode();
        }

        // parse command into the proper variables
        public override void decode()
        {
            Cond = bitsb(data, 31, 28);
            l = bitsb(data, 24, 24) == 1;
            immediate = bits(data, 23, 0);
            if (bitsb(data, 23, 23) == 1) immediate |= 0xff000000;
            immediate <<= 2;
        }

        // perform command on <regs> and <ram>
        public override void execute(Registers regs, Memory ram)
        {
            RAMReference = ram;
            registersReference = regs;
            if (l)
            {
                regs.WriteRegister(14, regs.ReadRegister(15)); // UNSURE! Current PC???
            }
            regs.WriteRegister(15, (uint)(regs.ReadRegister(15) + (int)immediate));
        }

        // Convert command to assembly string 
        public override string ToString()
        {
            return "b" + (l ? "l" : "") + " #" + (int)immediate;
        }
    }
}
