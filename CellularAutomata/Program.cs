﻿using System.Collections;

namespace CellularAutomata
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            //Rule 30R
            /*BitArray rule = new BitArray(16)
            {
                [1] = true,
                [2] = true,
                [3] = true,
                [4] = true,
                [8] = true,
                [13] = true,
                [14] = true,
                [15] = true
            };*/
            //Rule 110
            BitArray rule = new BitArray(8)
            {
                [1] = true,
                [2] = true,
                [3] = true,
                [5] = true,
                [6] = true
            };

            /*BitMatrix seed = new BitMatrix(2, 119)
            {
                [0, 60] = true,
                [1, 60] = true
            };*/

            BitMatrix seed = new BitMatrix(1, 9)
            {
                [0, 8] = true
            };

            int delay = 75;

            //CellularAutomata ca = new SecondOrderReversibleCA(rule, seed, delay);
            CellularAutomata ca = new ElementaryCA(rule, seed, delay);

            ca.SetupConsole();

            for (int i = 0; i < 500000; i++)
            {
                for (int j = 0; j < 500000; j++)
                {
                    ca.Draw();
                    ca.Iterate();
                }

                //ca.Modify(new []{"reverse"});
            }
        }
    }
}