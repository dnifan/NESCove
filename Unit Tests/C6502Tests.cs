﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NESCove.Core;
using NESCove.MOS6502;

namespace Unit_Tests
{
    [TestClass]
    public class C6502Tests
    {
        [TestMethod]
        [Priority(0)]
        public void C6502_Init()
        {
            /*
             * Create an unused test cpu. This will minimize the impact on the first test
             * due to looking up the CPU and Test memory classes. Providing a more accurate
             * computation time for the first test
             */
            var c = CreateTestCPU();
            c.Memory[0x00] = 0x00;
            c.Step();
        }

        [TestMethod]
        public void C6502_Execute_ImmediateValue_A()
        {
            var c = CreateTestCPU();
            c.Memory[0x00] = 0xA9;
            c.Memory[0x01] = 0xFF;
            c.Step();
            // We should probably assert all the registers (JL)
            Assert.AreEqual(c.State.RegA, 0xFF, "Loading FF into A using Immediate Addressing");
        }

        [TestMethod]
        public void C6502_Execute_AbsolouteValue_A()
        {
            var c = CreateTestCPU();
            c.Memory[0x16A0] = 0x13;
            c.Memory[0x00] = 0xAD;
            c.Memory[0x01] = 0xA0;
            c.Memory[0x02] = 0x16;
            c.Step();
            Assert.AreEqual(c.State.RegA, 0x13, "Loading 13 into A using absolute addressing");
        }

        [TestMethod]
        public void C6502_Execute_ZeroPage_A()
        {
            var c = CreateTestCPU();
            c.Memory[0xFC] = 0x14;
            c.Memory[0x00] = 0xA5;
            c.Memory[0x01] = 0xFC;
            c.Step();
            Assert.AreEqual(c.State.RegA, 0x14, "Loading 14 into A using Zero Page Addressing"); 
        }

        [TestMethod]
        public void C6502_Execute_Immediate_X()
        {
            var c = CreateTestCPU();
            c.Memory[0x00] = 0xA2;
            c.Memory[0x01] = 0x06;
            c.Step();
            Assert.AreEqual(c.State.RegX, 0x06, "Loading 06 into X using Immediate Addressing");
        }

        [TestMethod]
        public void C6502_Execute_PreIndexed_Indirect_A()
        {
            var c = CreateTestCPU();
            c.Memory[0x46] = 0x05;
            c.Memory[0x47] = 0x20;
            c.Memory[0x2005] = 0xFF;
            c.Memory[0x00] = 0xA1;
            c.Memory[0x01] = 0x46;
            c.Step();
            Assert.AreEqual(c.State.RegA, 0xFF, "Loading FF into A using Pre-Indexed Indirect Addressing");
        }

        [TestMethod]
        public void C6502_Execute_Immediate_Y()
        {
            var c = CreateTestCPU();
            c.Memory[0x00] = 0xA0;
            c.Memory[0x01] = 0x04;
            c.Step();
            Assert.AreEqual(c.State.RegY, 0x04, "Loading 04 into Y using Immediate Addressing");
        }

        [TestMethod]
        public void C6502_Execute_PostIndexed_Indirect_A()
        {
            var c = CreateTestCPU();
            c.Memory[0x48] = 0x19;
            c.Memory[0x49] = 0x32;
            c.Memory[0x3219] = 0xCF;
            c.Memory[0x00] = 0xB1;
            c.Memory[0x01] = 0x48;
            c.Step();
            Assert.AreEqual(c.State.RegA, 0xCF, "Loading CF into A using Post-Indexed Indirect Addressing");
        }

        [TestMethod]
        public void C6502_Execute_ZeroPage_Indexed_A()
        {
            var c = CreateTestCPU();
            c.Memory[0x00] = 0xA2;
            c.Memory[0x01] = 0xDF;
            c.Memory[0xDF] = 0xFE;
            c.Memory[0x02] = 0xB5;
            c.Memory[0x03] = 0x00;
            c.Step(2);
            Assert.AreEqual(c.State.RegA, 0xFE, "Loading FE into A using Zero-Page Indexed Addressing");
        }

        [TestMethod]
        public void C6502_Execute_Absolute_XA()
        {
            var c = CreateTestCPU();
            c.Memory[0x00] = 0xA2;
            c.Memory[0x01] = 0xFF;

            c.Memory[0x10FF] = 0xFB;
            c.Memory[0x02] = 0xBD;
            c.Memory[0x03] = 0x00;
            c.Memory[0x04] = 0x10;
            c.Step(2);
            Assert.AreEqual(c.State.RegA, 0xFB, "Loading FB into A using Absolute X Addressing");
        }

        [TestMethod]
        public void C6502_Execute_Absolute_YA()
        {
            var c = CreateTestCPU();
            c.Memory[0x00] = 0xA0;
            c.Memory[0x01] = 0xFF;

            c.Memory[0x11FF] = 0xFD;
            c.Memory[0x02] = 0xB9;
            c.Memory[0x03] = 0x00;
            c.Memory[0x04] = 0x11;
            c.Step(2);
            Assert.AreEqual(c.State.RegA, 0xFD, "Loading FD into A using Absolute Y Addressing");
        }

        [TestMethod]
        public void C6502_Execute_STA_Absolute()
        {
            // Store value of A (0xFF) in #$DEAD.
            IBasicCPU<byte, byte, ushort> c = CreateTestCPU();
            c.Memory[0x00] = 0xA9;
            c.Memory[0x01] = 0xFF;
            c.Memory[0x02] = 0x8D; // lol smiley face.
            c.Memory[0x03] = 0xAD;
            c.Memory[0x04] = 0xDE;

            c.Step(2);
            Assert.AreEqual(c.State.RegA, 0xFF, "Loading value of A (0xFF) into memory @ address 0xDEAD.");
        }

        [TestMethod]
        public void C6502_Execute_STA_PreIndexedIndirect()
        {
            // Store value of A (0xFF) in pointer at location #$2005.
            var c = CreateTestCPU();
            c.Memory[0x46] = 0x05;
            c.Memory[0x47] = 0x20;
            c.Memory[0x2005] = 0xCC;
            c.Memory[0x2006] = 0xCC;

            c.Memory[0x00] = 0xA9;
            c.Memory[0x01] = 0xFF;
            c.Memory[0x02] = 0x81;
            c.Memory[0x03] = 0x46;
            c.Step(2);

            Assert.AreEqual(c.State.RegA, 0xFF, "Loading value of A (0xFF) into memory @ pointer at address 0x2005.");
        }

        [TestMethod]
        public void C6502_Execute_STX_Absolute()
        {
            // Store value of X (0xFF) in #$DEAD.
            IBasicCPU<byte, byte, ushort> c = CreateTestCPU();
            c.Memory[0x00] = 0xA2;
            c.Memory[0x01] = 0xFF;
            c.Memory[0x02] = 0x8E; 
            c.Memory[0x03] = 0xAD;
            c.Memory[0x04] = 0xDE;

            c.Step(2);
            Assert.AreEqual(c.State.RegX, 0xFF, "Loading value of X (0xFF) into memory @ address 0xDEAD.");
        }

        [TestMethod]
        public void C6502_Execute_STY_Absolute()
        {
            // Store value of Y (0xFF) in #$DEAD.
            IBasicCPU<byte, byte, ushort> c = CreateTestCPU();
            c.Memory[0x00] = 0xA0;
            c.Memory[0x01] = 0xFF;
            c.Memory[0x02] = 0x8C; 
            c.Memory[0x03] = 0xAD;
            c.Memory[0x04] = 0xDE;

            c.Step(2);
            Assert.AreEqual(c.State.RegY, 0xFF, "Loading value of X (0xFF) into memory @ address 0xDEAD.");
        }

        private IBasicCPU<byte, byte, ushort> CreateTestCPU()
        {
            return (IBasicCPU<byte, byte, ushort>)new C6502();
        }


    }
}
