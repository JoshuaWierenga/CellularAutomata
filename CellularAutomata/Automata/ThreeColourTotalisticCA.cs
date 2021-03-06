﻿using System;
using System.Collections.Generic;
using System.Drawing;
using CellularAutomata.Devices.BaseDevices;

namespace CellularAutomata.Automata
{
    public class ThreeColourTotalisticCa : CellularAutomata
    {
        //Three Colour Totalistic CA runs in base 3
        private const int CAbase = 3;

        //Totalistic ca needs 2 rows, last row + current
        private const uint StateHeight = 2;

        //Number of possible inputs, sets required rule length, 3*colournum-2, i.e. 3*3-2 = 7
        private const int InputCount = 7;

        //Position to store seed in, Stores seed in current row
        private const uint SeedStartRow = 1;

        //Stores colours used for cells
        public override ConsoleColor[] Colours => new[]
        {
            //Used for cells with a value of 0
            ConsoleColor.White,
            //Used for cells with a value of 1
            ConsoleColor.DarkGray,
            //Used for cells with a value of 2
            ConsoleColor.Black
        };

        //Built in rules
        private static readonly Dictionary<string, int[]> DefaultRules = new Dictionary<string, int[]>
        {
            {"Code 777", new[]{0, 1, 2, 1, 0, 0, 1}},
            {"Manual Rule", null}
        };

        //Built in seeds
        private static readonly Dictionary<string, Dictionary<Point, int>> DefaultSeeds = new Dictionary<string, Dictionary<Point, int>>
        {
            {"Single 1 Top Left", new Dictionary<Point, int> {{new Point(0, 1), 1}}},
            {"Single 1 Top Middle", new Dictionary<Point, int> {{new Point(MaxSeedSize/2, 1), 1}}},
            {"Single 1 Top Right", new Dictionary<Point, int> {{new Point(MaxSeedSize, 1), 1}}},
            {"Single 2 Top Left", new Dictionary<Point, int> {{new Point(0, 1), 2}}},
            {"Single 2 Top Middle", new Dictionary<Point, int> {{new Point(MaxSeedSize/2, 1), 2}}},
            {"Single 2 Top Right", new Dictionary<Point, int> {{new Point(MaxSeedSize, 1), 2}}},
            {"Manual Seed", null}
        };

        //Sets up Three Colour Totalistic CA, device allows control of hardware other than the console
        public ThreeColourTotalisticCa(Device device) : base(StateHeight, InputCount, SeedStartRow, device, DefaultRules, CAbase, DefaultSeeds) {}

        //Sets up Three Colour Totalistic CA, rule controls output for input neighbourhoods and must be a 7 bit binary number, seed controls initial ca state
        //and must be a MaxSeedSize digit binary number and device allows control of hardware other than the console
        public ThreeColourTotalisticCa(int[] rule, int[,] seed, int delay, Device device) : base(StateHeight, InputCount, SeedStartRow, device, rule, seed, delay) {}

        //Find next row by applying rule to previous row
        //takes each group of 3 bits in current row, adds them and finds subrule that matches them and stores outputs as new row 
        public override void Iterate()
        {
            if (!Running)
            {
                return;
            }

            base.Iterate();

            for (uint i = 1; i < State.GetLength(1) - 1; i++)
            {
                //Add cells to find total for previous cells
                int total = State[0, i - 1] + State[0, i] + State[0, i + 1];

                //Set new cell to rule for total
                State[1, i] = Rule[total];
            }
        }

        //Draws last row + current as squares by using the unicode block elements
        public override void Draw()
        {
            //Should only draw every second iteration, waits until after first iteration as seed data only contains 1 row
            if (TimesRan % 2 == 0)
            {
                return;
            }

            base.Draw();
        }
    }
}