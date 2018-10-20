using System;
using System.Collections.Generic;
using System.Drawing;

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

        //Stores colours used for cells
        private static readonly ConsoleColor[] DefaultColours = {
            //Used for cells with a value of 0
            ConsoleColor.White,
            //Used for cells with a value of 1
            ConsoleColor.Black
        };

        private static readonly Dictionary<string, int[]> DefaultRules = new Dictionary<string, int[]>
        {
            {"Rule 102", new[]{0, 1, 1, 0, 0, 1, 1, 0}},
            {"Rule 110", new[]{0, 1, 1, 1, 0, 1, 1, 0}},
            {"Manual Rule", null}
        };

        private static readonly Dictionary<string, Dictionary<Point, int>> DefaultSeeds = new Dictionary<string, Dictionary<Point, int>>
        {
            {"Single Top Left", new Dictionary<Point, int> {{new Point(0, 1), 1}}},
            {"Single Top Middle", new Dictionary<Point, int> {{new Point(MaxSeedSize/2, 1), 1}}},
            {"Single Top Right", new Dictionary<Point, int> {{new Point(MaxSeedSize, 1), 1}}},
            {"Manual Seed", null}
        };

        public ElementaryCa(Device.Device device) : base(StateHeight, InputCount, SeedStartRow, device, DefaultRules, CAbase, DefaultSeeds, DefaultColours) {}

        //Rule must be an 8 digit binary number and seed must be a binary number that is shorter than current console width
        public ElementaryCa(int[] rule, int[,] seed, int delay, Device.Device device) : base(StateHeight, InputCount, SeedStartRow, device, rule, seed, delay, DefaultColours) {}

        //Find next row by applying rule to previous row
        public override void Iterate()
        {
            if (!Running)
            {
                return;
            }

            base.Iterate();

            for (uint i = 1; i < State.GetLength(1) - 1; i++)
            {
                //Concatenate ints and convert to binary
                string total = State[0, i - 1].ToString() + State[0, i] + State[0, i + 1];
                int totalBinary = Convert.ToInt32(total, CAbase);

                //Set new cell to rule for totalBinary
                State[1, i] = Rule[totalBinary];
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