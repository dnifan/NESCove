﻿using NESCove.MOS6502.Addressing;
using System;

namespace NESCove.MOS6502.Opcodes
{
    public class STA : OpcodeBase
    {
        public STA(IAddressingType addressing) 
            : base(addressing)
        {

        }

        public override int Execute(C6502 cpu, byte operand)
        {
            ExecutionState state = cpu.State as ExecutionState;
            if (state == null)
                throw new Exception("Something BAAAADD happened");

            cpu.Memory[AddressingType.GetAddress(cpu, state.Parameter)] = state.RegA;

            // I don't know what the fuck this is but it is 2 everywhere (DS).
            return 2;
        }
    }
}
