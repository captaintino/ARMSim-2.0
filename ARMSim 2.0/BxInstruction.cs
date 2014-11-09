using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    class BxInstruction : Instruction
    {
        uint progc;
        ushort Rm;

        public BxInstruction(uint data, uint progc)
        {
            this.data = data;
            this.progc = progc;
            this.decode();
        }

        // parse command into the proper variables
        public override void decode()
        {
            Cond = bitsb(data, 31, 28);
            Rm = bitsb(data, 3, 0);
        }

        // perform command on <regs> and <ram>
        public override void execute(CPU cpu)
        {
            registersReference = cpu.registers;
            registersReference.WriteRegister(15, (uint)((registersReference.ReadRegister(Rm) & 0xFFFFFFFE)));
        }

        // Convert command to assembly string 
        public override string ToString()
        {
            return "bx" + conditional() + " r" + Rm;
        }
    }
}
