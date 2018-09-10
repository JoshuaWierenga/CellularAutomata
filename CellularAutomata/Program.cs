using System.Collections;
using System.Threading;

namespace CellularAutomata
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            BitArray rule = new BitArray(8)
            {
                [1] = true,
                [2] = true,
                [3] = true,
                [5] = true,
                [6] = true
            };

            BitArray seed = new BitArray(110) {[109] = true};

            CellularAutomata ca = new ElementaryCA(rule, seed);
            ca.SetupConsole();

            for (int i = 0; i < 3000; i++)
            {
                if (i % 2 == 1)
                {
                    Thread.Sleep(75);
                    ca.Draw();
                }

                ca.Iterate();
            }
        }
    }
}