﻿using System;
using System.Collections;

namespace CellularAutomata
{
    public class ElementaryCA : CellularAutomata
    {
        //Elementary ca needs 2 rows, last row + current
        private const uint CAHeight = 2;

        //Number of possible inputs, sets required rule length
        private const uint InputCount = 8;

        //Position to store seed in, Stores seed in current row
        private const uint SeedPosition = 1;

        //Rule must be an 8 digit binary number and seed must be a binary number that is shorter than max chars on console row
        public ElementaryCA(BitArray rule, BitMatrix seed, int delay) 
            : base(CAHeight, InputCount, SeedPosition, rule, seed, delay) {}

        //Find next row by applying rule to previous row
        public override void Iterate()
        {
            base.Iterate();

            for (uint i = 1; i < State.ColumnCount - 1; i++)
            {
                bool previousBit = State[0, i - 1];
                bool currentBit = State[0, i];
                bool nextBit = State[0, i + 1];

                //000
                if (!previousBit && !currentBit && !nextBit)
                {
                    State[1, i] = Rule[0];
                }
                //001
                else if (!previousBit && !currentBit)
                {
                    State[1, i] = Rule[1];
                }
                //010
                else if (!previousBit && !nextBit)
                {
                    State[1, i] = Rule[2];
                }
                //011
                else if (!previousBit)
                {
                    State[1, i] = Rule[3];
                }
                //100
                else if (!currentBit && !nextBit)
                {
                    State[1, i] = Rule[4];
                }
                //101
                else if (!currentBit)
                {
                    State[1, i] = Rule[5];
                }
                //110
                else if (!nextBit)
                {
                    State[1, i] = Rule[6];
                }
                //111
                else
                {
                    State[1, i] = Rule[7];
                }
            }
        }

        //Draws last row + current as squares by using the unicode block elements
        public override void Draw()
        {
            //Should only draw every second iteration, waits until second iteration as seed data only contains 1 row
            if (TimesRan % 2 == 0)
            {
                return;
            }

            base.Draw();
        }

        public override void SetupConsole()
        {
            //TODO allow changing cell colour
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.CursorVisible = false;
            Console.Clear();
        }

        public override void Modify(string[] arguments)
        {
            throw new NotImplementedException();
        }
    }
}