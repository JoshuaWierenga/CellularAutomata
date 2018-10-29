using System;
using System.Collections.Generic;
using System.Drawing;
using CellularAutomata.Devices.BaseDevices;

namespace CellularAutomata.Automata
{
    public class SecondOrderReversibleCa : CellularAutomata
    {
        //Second Order Reversible CA runs in base 2
        private const int CAbase = 2;

        //Reversible ca needs 3 rows, 2nd previous row, previous row and current
        private const uint StateHeight = 3;

        //Number of possible inputs, sets required rule length
        private const int InputCount = 16;

        //Position to start storing seed in, stores seed in previous row + current row,
        private const uint SeedStartRow = 1;

        //Built in rules
        private static readonly Dictionary<string, int[]> DefaultRules = new Dictionary<string, int[]>
        {
            {"Rule 57630(30R)", new[]{0, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1}},
            {"Manual Rule", null}
        };

        //Built in seeds
        private static readonly Dictionary<string, Dictionary<Point, int>> DefaultSeeds = new Dictionary<string, Dictionary<Point, int>>
        {
            {"2 Stacked Top Left", new Dictionary<Point, int> {{new Point(0, 1), 1}, {new Point(0, 2), 1}}},
            {"2 Stacked Top Middle", new Dictionary<Point, int> {{new Point(MaxSeedSize/2, 1), 1}, {new Point(MaxSeedSize/2, 2), 1}}},
            {"2 Stacked Top Right", new Dictionary<Point, int> {{new Point(MaxSeedSize, 1), 1}, {new Point(MaxSeedSize, 2), 1}}},
            {"Manual Seed", null}
        };

        //Sets up Second Order Reversible CA, device allows control of hardware other than the console
        public SecondOrderReversibleCa(Device device) : base(StateHeight, InputCount,SeedStartRow, device, DefaultRules, CAbase, DefaultSeeds) {}

        //Sets up Second Order Reversible CA, rule controls output for input neighbourhoods and must be a 16 digit binary number, seed controls initial ca state
        //and must be a MaxSeedSize digit binary number and device allows control of hardware other than the console
        public SecondOrderReversibleCa(int[] rule, int[,] seed, int delay, Device device) : base(StateHeight, InputCount, SeedStartRow, device, rule, seed, delay) {}

        //Find next row by applying rule to previous + current rows
        //takes each group of 3 bits in current row + bit drectly above in previous row and finds subrule that matches them and stores outputs as new row 
        public override void Iterate()
        {
            if (!Running)
            {
                return;
            }

            base.Iterate();

            for (uint i = 1; i < State.GetLength(1) - 1; i++)
            {
                //Concatenate ints and convert to decimal
                string totalBinary = State[0, i].ToString() + State[1, i - 1] + State[1, i] + State[1, i + 1];
                int total = Convert.ToInt32(totalBinary, CAbase);

                //Set new cell to rule for total
                State[1, i] = Rule[total];
            }
        }

        //Draws last row + current as squares by using the unicode block elements
        public override void Draw()
        {
            //Should only draw every second iteration, runs before first iteration as seed data contains 2 rows
            if (TimesRan % 2 == 1)
            {
                return;
            }

            base.Draw();
        }

        //Reverses CA by moving previous row after current row
        private void Reverse()
        {
            for (uint i = 0; i < State.GetLength(1); i++)
            {
                int backup = State[2, i];
                State[2, i] = State[1, i];
                State[1, i] = backup;
            }

            TimesRan++;
            Iterate();
        }
    }
}