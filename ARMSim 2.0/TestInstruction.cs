using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ARMSim_2._0
{
    class TestInstruction
    {
        public void RunTests()
        {
            Console.WriteLine("Simulator: Tests: Instruction: Testing Instructions");
            Registers regs = new Registers(0);
            Memory ram = new Memory();
            CPU cpu = new CPU(ram, regs);
            regs.WriteRegister(3, 100);
            regs.WriteRegister(4, 1);
            Console.WriteLine("Simulator: Tests: Instruction: Testing Data Processing Instructions");

                Console.WriteLine("Simulator: Tests: Instruction: Testing MOV Instruction");
                DPInstruction dp = new DPInstruction(0xE1A05003u);
                Debug.Assert(dp.ToString() == "mov r5, r3");
                dp.execute(regs, ram);
                Debug.Assert(regs.ReadRegister(5) == 100);

                dp = new DPInstruction(0xE1A05083u);
                Debug.Assert(dp.ToString() == "mov r5, r3, lsl #1");
                dp.execute(regs, ram);
                Debug.Assert(regs.ReadRegister(5) == 200);

                dp = new DPInstruction(0xE3A05180u);
                Debug.Assert(dp.ToString() == "mov r5, #32");
                dp.execute(regs, ram);
                Debug.Assert(regs.ReadRegister(5) == 32);

                dp = new DPInstruction(0xE1A05413u);
                Debug.Assert(dp.ToString() == "mov r5, r3, lsl r4");
                dp.execute(regs, ram);
                Debug.Assert(regs.ReadRegister(5) == 200);
                Console.WriteLine("Simulator: Tests: Instruction: MOV Instruction Test Passed");

                Console.WriteLine("Simulator: Tests: Instruction: Testing Various Data Processing Instructions");
                Instruction ins = cpu.decode(0xE3E00002);
                Debug.Assert(ins.ToString() == "mvn r0, #2");
                cpu.execute(ins);
                Debug.Assert(regs.ReadRegister(0) == ~2u);

                ins = cpu.decode(0xE2440001);
                Debug.Assert(ins.ToString() == "sub r0, r4, #1");
                cpu.execute(ins);
                Debug.Assert(regs.ReadRegister(0) == 0);

                ins = cpu.decode(0xE2640002);
                Debug.Assert(ins.ToString() == "rsb r0, r4, #2");
                cpu.execute(ins);
                Debug.Assert(regs.ReadRegister(0) == 1);

                ins = cpu.decode(0xE20400FF);
                Debug.Assert(ins.ToString() == "and r0, r4, #255");
                cpu.execute(ins);
                Debug.Assert(regs.ReadRegister(0) == 1);

                ins = cpu.decode(0xE3800012);
                Debug.Assert(ins.ToString() == "orr r0, r0, #18");
                cpu.execute(ins);
                Debug.Assert(regs.ReadRegister(0) == 19);

                ins = cpu.decode(0xE2200020);
                Debug.Assert(ins.ToString() == "eor r0, r0, #32");
                cpu.execute(ins);
                Debug.Assert(regs.ReadRegister(0) == 51);
                Console.WriteLine("Simulator: Tests: Instruction: Various Data Processing Instructions Test Passed");

            regs.WriteRegister(4, 2);

                Console.WriteLine("Simulator: Tests: Instruction: Testing MUL Instruction");
                ins = cpu.decode(0xE0000493);
                Debug.Assert(ins.ToString() == "mul r0, r3, r4");
                cpu.execute(ins);
                Debug.Assert(regs.ReadRegister(0) == 200);
                Console.WriteLine("Simulator: Tests: Instruction: MUL Instruction Test Passed");


            ram.WriteWord(224u, 100);
            ram.WriteWord(200u, 10);
            ram.WriteWord(228u, 110);
            ram.WriteByte(3u, 3);
            regs.WriteRegister(0, 0);

                Console.WriteLine("Simulator: Tests: Instruction: Testing LOAD Instruction");
                LSInstruction lp = new LSInstruction(0xe5954018);
                Debug.Assert(lp.ToString() == "ldr r4, [r5, #24]");
                lp.execute(regs, ram);
                Debug.Assert(regs.ReadRegister(4) == 100);

                lp = new LSInstruction(0xe4954018);
                Debug.Assert(lp.ToString() == "ldr r4, [r5], #24");
                lp.execute(regs, ram);
                Debug.Assert(regs.ReadRegister(4) == 10);
                Debug.Assert(regs.ReadRegister(5) == 224);

                regs.WriteRegister(5, 200);

                lp = new LSInstruction(0xe5b5401c);
                Debug.Assert(lp.ToString() == "ldr r4, [r5, #28] !");
                lp.execute(regs, ram);
                Debug.Assert(regs.ReadRegister(4) == 110);
                Debug.Assert(regs.ReadRegister(5) == 228);

                ins = cpu.decode(0xE5D04003);
                Debug.Assert(ins.ToString() == "ldrb r4, [r0, #3]");
                cpu.execute(ins);
                Debug.Assert(regs.ReadRegister(4) == 3);
                Console.WriteLine("Simulator: Tests: Instruction: LOAD Instruction Test Passed");

            regs.WriteRegister(0, 12);
            regs.WriteRegister(10, 40);
            ram.WriteWord(64, 0);

                Console.WriteLine("Simulator: Tests: Instruction: Testing STORE Instruction");
                LSInstruction sp = new LSInstruction(0xe58a0018);
                Debug.Assert(sp.ToString() == "str r0, [r10, #24]");
                sp.execute(regs, ram);
                Debug.Assert(ram.ReadWord(64) == 12);

                sp = new LSInstruction(0xe48a0018);
                Debug.Assert(sp.ToString() == "str r0, [r10], #24");
                sp.execute(regs, ram);
                Debug.Assert(ram.ReadWord(40) == 12);

                sp = new LSInstruction(0xe5a00018);
                Debug.Assert(sp.ToString() == "str r0, [r0, #24] !");
                sp.execute(regs, ram);
                Debug.Assert(ram.ReadWord(36) == 12);
                Debug.Assert(regs.ReadRegister(0) == 36);

                ins = cpu.decode(0xE5C00003);
                Debug.Assert(ins.ToString() == "strb r0, [r0, #3]");
                cpu.execute(ins);
                Debug.Assert(ram.ReadByte(39) == 36);
                Console.WriteLine("Simulator: Tests: Instruction: STORE Instruction Test Passed");



                Console.WriteLine("Simulator: Tests: Instruction: Testing BRANCH Instruction");
                BrInstruction bp = new BrInstruction(0xea000001);
                Debug.Assert(bp.ToString() == "b #4");
                bp.execute(regs, ram);
                Debug.Assert(regs.ReadRegister(15) == 4);

                bp = new BrInstruction(0xebffffff);
                Debug.Assert(bp.ToString() == "bl #-4");
                bp.execute(regs, ram);
                Debug.Assert((int)regs.ReadRegister(15) == 0);
                Debug.Assert(regs.ReadRegister(14) == 4);
                Console.WriteLine("Simulator: Tests: Instruction: BRANCH Instruction Test Passed");

            regs.WriteRegister(2, 20);
            regs.WriteRegister(3, 10);

                Console.WriteLine("Simulator: Tests: Instruction: Testing MUL Instruction");
                MulInstruction mp = new MulInstruction(0xe0010293);
                Debug.Assert(mp.ToString() == "mul r1, r3, r2");
                mp.execute(regs, ram);
                Debug.Assert(regs.ReadRegister(1) == 200);
                Console.WriteLine("Simulator: Tests: Instruction: MUL Instruction Test Passed");

            regs.WriteRegister(1, 400);
            regs.WriteRegister(15, 11);
            regs.WriteRegister(14, 10);
            regs.WriteRegister(13, 9);
            regs.WriteRegister(12, 8);
            regs.WriteRegister(3, 2);

                Console.WriteLine("Simulator: Tests: Instruction: Testing LOAD/STORE MULTIPLE Instruction");
                LSMulInstruction lsmp = new LSMulInstruction(0xe8a1f008);
                Debug.Assert(lsmp.ToString() == "stmea r1!, {r3, r12, r13, r14, r15}");
                lsmp.execute(regs, ram);
                Debug.Assert(ram.ReadWord(416) == 2);
                Debug.Assert(ram.ReadWord(400) == 11);
                Debug.Assert(ram.ReadWord(404) == 10);
                Debug.Assert(ram.ReadWord(408) == 9);
                Debug.Assert(ram.ReadWord(412) == 8);
                Debug.Assert(regs.ReadRegister(1) == 416);

                regs.WriteRegister(1, 416);

                lsmp = new LSMulInstruction(0xe811f008);
                Debug.Assert(lsmp.ToString() == "ldmfa r1, {r3, r12, r13, r14, r15}");
                lsmp.execute(regs, ram);
                Debug.Assert(regs.ReadRegister(15) == 2);
                Debug.Assert(regs.ReadRegister(14) == 8);
                Debug.Assert(regs.ReadRegister(13) == 9);
                Debug.Assert(regs.ReadRegister(12) == 10);
                Debug.Assert(regs.ReadRegister(3) == 11);
                Debug.Assert(regs.ReadRegister(1) == 416);
                Console.WriteLine("Simulator: Tests: Instruction: LOAD/STORE MULTIPLE Instruction Test Passed");

            Console.WriteLine("Simulator: Tests: Instruction: All tests Passed");
        }
    }
}
