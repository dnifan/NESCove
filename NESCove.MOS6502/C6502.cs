﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NESCove.Core;
using NESCove.MOS6502.Opcodes;

namespace NESCove.MOS6502
{
    public class C6502
    {
        // Not sure where we should be putting these types of constants
        public const int PageSize = 256;

        // Special Purpose Registers
        public UInt16 ProgramCounter { get; set; }
        public Byte StackPointer { get; set; }      // Stack is at 0x0100 - 0x01FF. 
        public StatusFlags ProcessorStatus { get; set; }
        // General Purpose Registers
        public Byte RegA { get; set; }
        public Byte RegX { get; set; }
        public Byte RegY { get; set; }

        public IMemoryProvider Memory { get; set; }

        public bool IsFlagSet(StatusFlags flag)
        {
            return (ProcessorStatus & flag) != 0;
        }

        public void SetFlag(StatusFlags flag)
        {
            ProcessorStatus |= flag;
        }

        public void ResetFlag(StatusFlags flag)
        {
            ProcessorStatus &= ~flag;
        }

        public void Step(int iterations = 1)
        {
            // Test code for testing the MOS6502 processor (NES version)
            // Bank switching / MMC must be implemented later.
            if (iterations == 1)
            {
                byte opcode = Memory[ProgramCounter++];

                IOpcode opcodeHandler = OpcodeFactory.GetOpcode(opcode);
                if (opcodeHandler == null)
                    throw new Exception(string.Format("Opcode {0:X2} not implemented yet", opcode));

                ushort parameter = 0;
                if (opcodeHandler.AddressingType.ParameterSize.HasValue)
                {
                    // Hacky quick and dirty way to retrieve parameters. Note the endianness, might need to modify this later (DS)
                    // Hope this is satisfactory (JL)
                    var parameterSize = opcodeHandler.AddressingType.ParameterSize.Value;
                    parameter = DataHelper.CompositeInteger(Memory, ProgramCounter, parameterSize);
                    ProgramCounter += parameterSize;
                }
                opcodeHandler.Execute(this, parameter);
            }
            else if (iterations > 1)
            {
                for (int iteration = 0; iteration < iterations; iteration++)
                {
                    Step();
                }
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("=======================================\r\n");
            builder.AppendFormat("  PC = {0:X4}\tSP= {1:X2}\r\n", ProgramCounter, StackPointer);
            builder.AppendFormat("   A = {0:X2}\tX = {1:X2}\t\tY = {2:X2}\r\n", RegA, RegX, RegY);
            builder.AppendFormat("=======================================\r\n\r\n");
            return builder.ToString();
        }
    }
}
 