using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    class Operand2
    {
        ushort shiftAmount, shift, Rm, Rs; // register version variables
        ushort RoR; uint immediate; // Immediate variables
        ushort type;
        // 2 => immediate rotate : 1 => register shift : 0 => immediate shift
        static string[] shiftString = { "lsl", "asr", "lsr", "ror" };



        public Operand2(uint data, bool immediateRotate)
        {
            if (immediateRotate)
            {
                type = 2;
                RoR = Instruction.bitss(data, 11, 8);
                immediate = Instruction.bits(data, 7, 0);
            }
            else
            {
                if (Instruction.bitss(data, 4, 4) == 1)
                {
                    type = 1;
                    Rs = Instruction.bitss(data, 11, 8);
                }
                else
                {
                    type = 0;
                    shiftAmount = Instruction.bitss(data, 11, 7);
                }
                shift = Instruction.bitss(data, 6, 5);
                Rm = Instruction.bitss(data, 3, 0);
            }
        }

        public uint execute(Registers regs)
        {
            switch (type)
            {
                case 0:
                    return PerformShift(shiftAmount, shift, regs.ReadRegister(Rm));
                case 1:
                    return PerformShift((int)regs.ReadRegister(Rs), shift, regs.ReadRegister(Rm));
                case 2:
                    return (immediate >> (RoR * 2)) | (immediate << (32 - (RoR * 2)));
            }
            return 0;
        }

        public uint PerformShift(int shift, uint shiftType, uint Rm)
        {
            switch (shiftType)
            {
                case 0:// LSL
                    return Rm << shift;
                case 1:// ASR
                    return (uint)((int)(Rm) >> shift);
                case 2:// LSR
                    return Rm >> shift;
                case 4:// ROR
                    return (Rm >> shift % 32) | (Rm << (32 - shift % 32));
            }
            return 0;
        }

        public override string ToString()
        {
            switch (type)
            {
                case 0:
                    return "R" + Rm + (shiftAmount == 0 ? "" : ", " + shiftString[shift] + " #" + shiftAmount); 
                case 1:
                    return "R" + Rm + ", " + shiftString[shift] + " R" + Rs; 
                case 2:
                    return "#" + ((immediate >> (RoR * 2)) | (immediate << (32 - (RoR * 2)))).ToString();
            }
            return "";
        }

    }
}
