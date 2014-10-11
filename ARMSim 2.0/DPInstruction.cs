using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    class DPInstruction : Instruction
    {
        ushort opcode;
        bool s;
        Operand2 op2;

        public DPInstruction(uint data)
        {
            this.data = data;
            this.decode();
        }

        // parse command into the proper variables
        public override void decode()
        {
            Cond = bitsb(data, 31, 28);
            opcode = (ushort)bits(data, 24, 21);
            Rn = bitss(data, 19, 16);
            Rd = bitss(data, 15, 12);
            s = bitsb(data, 20, 20) == 1;
            i = bitsb(data, 25, 25) == 1;
            op2 = new Operand2(data, i);
        }

        // perform command on <regs> and <ram>
        public override void execute(Registers regs, Memory ram)
        {
            RAMReference = ram;
            registersReference = regs;
            switch (opcode)
            {
                case 0: // AND
                    break;
                case 1: // EOR
                    break;
                case 2: // SUB
                    break;
                case 3: // RSB
                    break;
                case 4: // ADD
                    break;
                case 5: // ADC
                    break;
                case 6: // SBC
                    break;
                case 7: // RSC
                    break;
                case 8: // TST
                    break;
                case 9: // TEQ
                    break;
                case 10: // CMP
                    break;
                case 11: // CMN
                    break;
                case 12: // ORR
                    break;
                case 13: // MOV
                    registersReference.WriteRegister(Rd, op2.execute(registersReference));
                    break;
                case 14: // BIC
                    break;
                case 15: // MVN
                    break;
            }
        }

        // Convert command to assembly string 
        public override string ToString()
        {
            switch (opcode)
            {
                case 0: // AND
                    break;
                case 1: // EOR
                    break;
                case 2: // SUB
                    break;
                case 3: // RSB
                    break;
                case 4: // ADD
                    break;
                case 5: // ADC
                    break;
                case 6: // SBC
                    break;
                case 7: // RSC
                    break;
                case 8: // TST
                    break;
                case 9: // TEQ
                    break;
                case 10: // CMP
                    break;
                case 11: // CMN
                    break;
                case 12: // ORR
                    break;
                case 13: // MOV
                    return "mov r" + Rd + ", " + op2.ToString();
                case 14: // BIC
                    break;
                case 15: // MVN
                    break;
            }
            return "nop";
        }
    }
}
