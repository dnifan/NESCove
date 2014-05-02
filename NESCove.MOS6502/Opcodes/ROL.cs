﻿using NESCove.MOS6502.Addressing;

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
            if ((result & 0x100) != 0)
                cpu.State.SetFlag((byte) StatusFlags.Carry);
            else
                cpu.State.ClearFlag((byte) StatusFlags.Carry);
            // TODO; Thank you Josh for removing the SetZero/SetNEgative again :p need to set ZERO/NEGATIVE even if it is not stored in A

            // Hacky but it will do the trick. Accumulator Addressing is an outcast.
            if (AddressingType is AccumulatorAddressing)
                cpu.State.RegA = (byte)result;
            else
                cpu.Memory[AddressingType.GetAddress(cpu, state.Parameter)] = (byte)result;

            return 2;
        }
    }
}
