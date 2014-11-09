using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    class MulInstruction : Instruction
    {
        ushort Rs, Rm;
        bool s, a, un;

        public MulInstruction(uint data)
        {
            this.data = data;
            this.decode();
        }

        // parse command into the proper variables
        public override void decode()
        {
            Cond = bitsb(data, 31, 28);
            Rd = bitss(data, 19, 16);
            Rn = bitss(data, 15, 12);
            Rs = bitss(data, 11, 8);
            Rm = bitss(data, 3, 0);
            s = bitsb(data, 20, 20) == 1;
            a = bitsb(data, 21, 21) == 1;
            un = bitsb(data, 22, 22) == 1;
        }

        // perform command on <regs> and <ram>
        public override void execute(CPU cpu)
        {
            registersReference = cpu.registers;
            if (bitsb(data, 23, 23) == 1) // Multiply (acc) long
            {

            }
            else if (un) // Unsigned multiply acc acc long
            {

            }
            else // Multiply (acc)
            {
                // IF S?
                registersReference.WriteRegister(Rd, registersReference.ReadRegister(Rm) * registersReference.ReadRegister(Rs));
            }
        }

        // Convert command to assembly string 
        public override string ToString()
        {
            if (bitsb(data, 23, 23) == 1) // Multiply (acc) long
            {

            }
            else if (un) // Unsigned multiply acc acc long
            {

            }
            else // Multiply (acc)
            {
                // IF S?
                return "mul" + conditional() + " r" + Rd + ", r" + Rm + ", r" + Rs;
            }
            return "nop";
        }
    }
}
