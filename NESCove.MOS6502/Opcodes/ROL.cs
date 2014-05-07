﻿using NESCove.Core;
using NESCove.MOS6502.Addressing;

namespace NESCove.MOS6502.Opcodes
{
    public class ROL : OpcodeBase
    {
        public ROL(IAddressingType addressing) : base(addressing)
        {
        }

        public override int Execute(C6502 cpu, byte operand)
        {
            var state = cpu.State as ExecutionState;

            int result = operand << 1;
            result |= (cpu.State.ProcessorStatus & (byte) StatusFlags.Carry);

            SetCarry(cpu, () => (result & 0x100) != 0);
            result &= 0xFF;
            SetNegative(cpu, () => Helper.IsSigned((byte)result));
            SetZero(cpu, () => result == 0);

            // Hacky but it will do the trick. Accumulator Addressing is an outcast.
            if (AddressingType is AccumulatorAddressing)
                cpu.State.RegA = (byte)result;
            else
                cpu.Memory[AddressingType.GetAddress(cpu, state.Parameter)] = (byte)result;

            return 2;
        }
    }
}
