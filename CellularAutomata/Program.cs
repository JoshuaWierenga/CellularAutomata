﻿namespace CellularAutomata
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            //Rule 30R
            /*int[] rule = new int[16];
            rule[1] = 1;
            rule[2] = 1;
            rule[3] = 1;
            rule[4] = 1;
            rule[8] = 1;
            rule[13] = 1;
            rule[14] = 1;
            rule[15] = 1;*/

            //Rule 110
            int[] rule = new int[8];
            rule[1] = 1;
            rule[2] = 1;
            rule[3] = 1;
            rule[5] = 1;
            rule[6] = 1;


            /*int[,] seed = new int[2, 119];
            seed[0, 60] = 1;
            seed[1, 60] = 1;*/

            int[,] seed = new int[1,30];
            seed[0, 29] = 1;

            int delay = 75;

            //Automata.CellularAutomata ca = new SecondOrderReversibleCA(rule, seed, delay);
            Automata.CellularAutomata ca = new ElementaryCA(rule, seed, delay);

            ca.SetupConsole();

            for (int i = 0; i < 500; i++)
            {
                for (int j = 0; j < 500; j++)
                {
                    ca.Draw();
                    ca.Iterate();
                }

                //ca.Modify(new []{"reverse"});
            }
        }
    }
}