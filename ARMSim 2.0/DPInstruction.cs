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
        public override void execute(CPU cpu)
        {
            registersReference = cpu.registers;
            switch (opcode)
            {
                case 0: // AND
                    registersReference.WriteRegister(Rd, registersReference.ReadRegister(Rn) & op2.execute(registersReference));
                    break;
                case 1: // EOR
                    registersReference.WriteRegister(Rd, registersReference.ReadRegister(Rn) ^ op2.execute(registersReference));
                    break;
                case 2: // SUB
                    registersReference.WriteRegister(Rd, registersReference.ReadRegister(Rn) - op2.execute(registersReference));
                    break;
                case 3: // RSB
                    registersReference.WriteRegister(Rd, op2.execute(registersReference) - registersReference.ReadRegister(Rn));
                    break;
                case 4: // ADD
                    registersReference.WriteRegister(Rd, registersReference.ReadRegister(Rn) + op2.execute(registersReference));
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
                    DoCompare(cpu);
                    break;
                case 11: // CMN
                    break;
                case 12: // ORR
                    registersReference.WriteRegister(Rd, registersReference.ReadRegister(Rn) | op2.execute(registersReference));
                    break;
                case 13: // MOV
                    registersReference.WriteRegister(Rd, op2.execute(registersReference)); // TESTED 1
                    break;
                case 14: // BIC
                    registersReference.WriteRegister(Rd, registersReference.ReadRegister(Rn) & (~op2.execute(registersReference)));
                    break;
                case 15: // MVN
                    registersReference.WriteRegister(Rd, ~op2.execute(registersReference));
                    break;
            }
        }

        // Convert command to assembly string 
        public override string ToString()
        {
            switch (opcode)
            {
                case 0: // AND
                    return "and" + conditional() + "  r" + Rd + ", r" + Rn + ", " + op2.ToString();
                case 1: // EOR
                    return "eor" + conditional() + "  r" + Rd + ", r" + Rn + ", " + op2.ToString();
                case 2: // SUB
                    return "sub" + conditional() + "  r" + Rd + ", r" + Rn + ", " + op2.ToString();
                case 3: // RSB
                    return "rsb" + conditional() + "  r" + Rd + ", r" + Rn + ", " + op2.ToString();
                case 4: // ADD
                    return "add" + conditional() + "  r" + Rd + ", r" + Rn + ", " + op2.ToString();
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
                    return "cmp" + conditional() + " r" + Rn + ", " + op2.ToString();
                case 11: // CMN
                    break;
                case 12: // ORR
                    return "orr" + conditional() + "  r" + Rd + ", r" + Rn + ", " + op2.ToString();
                case 13: // MOV
                    return "mov" + conditional() + "  r" + Rd + ", " + op2.ToString();
                case 14: // BIC
                    return "bic" + conditional() + "  r" + Rd + ", r" + Rn + ", " + op2.ToString();
                case 15: // MVN
                    return "mvn" + conditional() + "  r" + Rd + ", " + op2.ToString();
            }
            return "nop";
        }

        private void DoCompare(CPU cpu)
        {
            uint firstVal = registersReference.ReadRegister(Rn);
            uint secondVal = op2.execute(registersReference);
            uint subtracted = firstVal - secondVal;
            cpu.SetNFlag((subtracted & 0x80000000) != 0);
            cpu.SetZFlag(subtracted == 0);
            cpu.SetCFlag(secondVal <= firstVal);
            int sFirstVal = (int)firstVal;
            int sSecondVal = (int)secondVal;
            if (sFirstVal >= 0)
            {
                cpu.SetFFlag(sSecondVal < 0 ? sFirstVal - sSecondVal < 0 : false);
            }
            else
            {
                cpu.SetFFlag(sSecondVal >= 0 ? sFirstVal - sSecondVal > 0 : false);
            }
        }
    }
}
