using System;
using System.Collections.Generic;
using System.Drawing;
using CellularAutomata.Devices.BaseDevices;

namespace CellularAutomata.Automata
{
    public class ElementaryCa : CellularAutomata
    {
        //Elementary CA runs in base 2
        private const int CAbase = 2;

        //Elementary ca needs 2 rows, last row + current
        private const uint StateHeight = 2;

        //Number of possible inputs, sets required rule length
        private const int InputCount = 8;

        //Position to store seed in, 1 stores seed in current row
        private const uint SeedStartRow = 1;

        //Built in rules
        private static readonly Dictionary<string, int[]> DefaultRules = new Dictionary<string, int[]>
        {
            {"Rule 102", new[]{0, 1, 1, 0, 0, 1, 1, 0}},
            {"Rule 110", new[]{0, 1, 1, 1, 0, 1, 1, 0}},
            {"Manual Rule", null}
        };

        //Built in seeds
        private static readonly Dictionary<string, Dictionary<Point, int>> DefaultSeeds = new Dictionary<string, Dictionary<Point, int>>
        {
            {"Single Top Left", new Dictionary<Point, int> {{new Point(0, 1), 1}}},
            {"Single Top Middle", new Dictionary<Point, int> {{new Point(MaxSeedSize/2, 1), 1}}},
            {"Single Top Right", new Dictionary<Point, int> {{new Point(MaxSeedSize, 1), 1}}},
            {"Manual Seed", null}
        };

        //Sets up Elementary CA, device allows control of hardware other than the console
        public ElementaryCa(Device device) : base(StateHeight, InputCount, SeedStartRow, device, DefaultRules, CAbase, DefaultSeeds) {}

        //Sets up Elementary CA, rule controls output for input neighbourhoods and must be an 8 digit binary number, seed controls initial ca state
        //and must be a MaxSeedSize digit binary number and device allows control of hardware other than the console
        public ElementaryCa(int[] rule, int[,] seed, int delay, Device device) : base(StateHeight, InputCount, SeedStartRow, device, rule, seed, delay) {}

        //Find next row by applying rule to current row
        //takes each group of 3 bits in current row and finds subrule that matches them and stores outputs as new row 
        public override void Iterate()
        {
            if (!Running)
            {
                return;
            }

            //Base handles shifting state array back one row to create room for the new row and sets outer cells
            base.Iterate();

            for (uint i = 1; i < State.GetLength(1) - 1; i++)
            {
                //Concatenate ints and convert to decimal
                string totalBinary = State[0, i - 1].ToString() + State[0, i] + State[0, i + 1];
                int total = Convert.ToInt32(totalBinary, CAbase);

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

            //Base takes both rows of state array and sets background colour to match their values and draws them the the same cell
            //using unicode block chars
            base.Draw();
        }
    }
}