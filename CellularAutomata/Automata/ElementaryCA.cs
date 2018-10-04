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
        private const uint InputCount = 8;

        //Position to store seed in, 1 stores seed in current row
        private const uint SeedStartRow = 1;

        private static readonly Dictionary<string, int[]> DefaultRules = new Dictionary<string, int[]>
        {
            {"Rule 102", new[]{0, 1, 1, 0, 0, 1, 1, 0}},
            {"Rule 110", new[]{0, 1, 1, 1, 0, 1, 1, 0}},
            {"Manual Rule", new[]{0} }
        };

        private static readonly Dictionary<string, Point[]> DefaultSeeds = new Dictionary<string, Point[]>
        {
            {"Single Top Left", new[] {new Point(0, 1)}},
            {"Single Top Middle", new []{ new Point((Console.WindowWidth - 2)/2, 1)}},
            {"Single Top Right", new []{ new Point(Console.WindowWidth - 2, 1)}}
        };

        public ElementaryCa() : base(StateHeight, InputCount, DefaultRules, CAbase, DefaultSeeds) {}

        //Rule must be an 8 digit binary number and seed must be a binary number that is shorter than current console width
        public ElementaryCa(int[] rule, int[,] seed, int delay) : base(StateHeight, InputCount, SeedStartRow, rule, seed, delay) {}

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
                //Concatinate ints and convert to binary
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