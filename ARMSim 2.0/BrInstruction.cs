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
        uint immediate, progc;

        public BrInstruction(uint data, uint progc)
        {
            this.data = data;
            this.progc = progc;
            this.decode();
        }

        // parse command into the proper variables
        public override void decode()
        {
            Cond = bitsb(data, 31, 28);
            l = bitsb(data, 24, 24) == 1;
            immediate = bits(data, 23, 0) + 1;
            if (bitsb(data, 23, 23) == 1) immediate |= 0xff000000;
            immediate <<= 2;
        }

        // perform command on <regs> and <ram>
        public override void execute(CPU cpu)
        {
            RAMReference = cpu.ram;
            registersReference = cpu.registers;
            if (l)
            {
                registersReference.WriteRegister(14, registersReference.ReadRegister(15) - 4); // UNSURE! -4 added to match correct trace log???
            }
            registersReference.WriteRegister(15, (uint)(registersReference.ReadRegister(15) + (int)immediate));
        }

        // Convert command to assembly string 
        public override string ToString()
        {
            return "b" + (l ? "l" : "") + conditional() + String.Format(" #{0:X8}", (uint)((int)(immediate) + progc + 4));
        }
    }
}
