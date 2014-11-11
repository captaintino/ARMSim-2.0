using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    public abstract class Instruction
    {
        public ushort Rn, Rd, Cond;
        public uint data;
        public bool i;
        public Registers registersReference;
        public Memory RAMReference;

        static private string[] conditionMnemonic = { "EQ", "NE", "CS", "CC", "MI", "PL", "VS", "VC", "HI", "LS", "GE", "LT", "GT", "LE", "", ""};

        public abstract void decode();
        public abstract void execute(CPU cpu);
        public virtual String ToString() { return ""; }


        // Identify type of instruction and create/return instance of it
        public static Instruction InstructionFactory(uint data, uint progc = 0){
            if (bitsb(data, 27, 24) == 15) return new SWIInstruction(data);
            if (bitsb(data, 27, 24) == 0 && bitsb(data, 7, 4) == 9) return new MulInstruction(data);
            if (bitsb(data, 27, 20) == 18 && bitsb(data, 7, 4) == 1) return new BxInstruction(data, progc);
            if(bitsb(data, 27, 23) == 2 && bitsb(data,7,4) == 0) return new MSInstruction(data);
            if(bitsb(data, 27, 23) == 6 && bitsb(data,21,20) == 2) return new MSInstruction(data);
            uint itype = bits(data, 27, 26);
            switch (itype)
            {
                case 0:
                    // Data Processing
                    return new DPInstruction(data);
                case 1:
                    // Load/Store
                    return new LSInstruction(data);
                case 2:
                    // Load/Store Multiple
                    if (bitsb(data, 25, 25) == 0) return new LSMulInstruction(data);
                    // Branch
                    return new BrInstruction(data, progc);
                default:
                    //Should never happen, special cases checked BEFORE
                    break;
            }
            return null;
        }
        // return bits <start> through <end> in <data>
        static public uint bits(uint data, int start, int end)
        {
            return (data << (31 - start)) >> (31 + end - start);
        }
        // return bits <start> through <end> in <data>
        static public ushort bitss(uint data, int start, int end)
        {
            return (ushort)((data << (31 - start)) >> (31 + end - start));
        }
        // return bits <start> through <end> in <data>
        static public byte bitsb(uint data, int start, int end)
        {
            return (byte)((data << (31 - start)) >> (31 + end - start));
        }

        public string conditional()
        {
            return conditionMnemonic[Cond];
        }
    }
}
