using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMSim_2._0
{
    abstract public class Instruction
    {
        public ushort Rn, Rd, Cond;
        public uint data;
        public bool i;
        public Registers registersReference;
        public Memory RAMReference;

        public virtual void decode() { }
        public virtual void execute() { }
        public virtual void execute(Registers regs, Memory ram) { }
        public virtual String ToString() { return ""; }



        public static Instruction InstructionFactory(uint data){
            // check special cases
            uint itype = bits(data, 27, 26);
            switch (itype)
            {
                case 0:
                    // Data Processing
                    return new DPInstruction(data);
                case 1:
                    // Load/Store
                    break;
                case 2:
                    // Branch?
                    break;
                default:
                    //Should never happen, special cases checked BEFORE
                    break;
            }
            return null;
        }
        static public uint bits(uint data, int start, int end)
        {
            return (data << (31 - start)) >> (31 + end - start);
        }
        static public ushort bitss(uint data, int start, int end)
        {
            return (ushort)((data << (31 - start)) >> (31 + end - start));
        }
        static public byte bitsb(uint data, int start, int end)
        {
            return (byte)((data << (31 - start)) >> (31 + end - start));
        }
    }
}
