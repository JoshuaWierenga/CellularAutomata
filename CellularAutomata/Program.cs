using CellularAutomata.Automata;

namespace CellularAutomata
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            int delay = 75;

            //Rule 30R
            /*int[] rule = new int[16];
            rule[1] = 1;
            rule[2] = 1;
            rule[3] = 1;
            rule[4] = 1;
            rule[8] = 1;
            rule[13] = 1;
            rule[14] = 1;
            rule[15] = 1;

            int[,] seed = new int[2, 119];
            seed[0, 60] = 1;
            seed[1, 60] = 1;

            Automata.CellularAutomata ca = new SecondOrderReversibleCa(rule, seed, delay);*/

            Automata.CellularAutomata ca = new ElementaryCa();

            //Code 177
            /*int[] rule = new int[7];
            rule[1] = 2;
            rule[2] = 1;
            rule[4] = 2;

            int[,] seed = new int[1, 119];
            seed[0, 60] = 2;

            Automata.CellularAutomata ca = new ThreeColourTotalisticCa(rule, seed, delay);*/

            //ca.Modify(Modification.Colour, ConsoleColor.White, ConsoleColor.DarkBlue, ConsoleColor.Magenta);

            ca.SetupConsole();

            for (int i = 0; i < 500; i++)
            {
                for (int j = 0; j < 23; j++)
                {
                    ca.Draw();
                    ca.Iterate();
                }

                //ca.Modify(Modification.Direction);
            }
        }
    }
}