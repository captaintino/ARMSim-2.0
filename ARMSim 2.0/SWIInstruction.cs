﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    class SWIInstruction : Instruction
    {
        uint number;

        public SWIInstruction(uint data)
        {
            this.data = data;
            this.decode();
        }

        // parse command into the proper variables
        public override void decode()
        {
            Cond = bitsb(data, 31, 28);
            number = bits(data, 23, 0);
        }

        // perform command on <regs> and <ram>
        public override void execute(CPU cpu)
        {
            registersReference = cpu.registers;
            switch (number)
            {
                case 0:
                    cpu.StopComputer();
                    break;
                case 17:
                    registersReference.SwitchModes(Global.SUPERVISORMODE);
                    cpu.SetModeBits(Global.SUPERVISORMODE);
                    break;
            }
            cpu.StopComputer(); // To avoid infinite loops REMOVE LATER
        }

        // Convert command to assembly string 
        public override string ToString()
        {
            return "swi" +  conditional() + " #" + number;
        }
    }
}
