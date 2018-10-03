using System;
using System.Collections.Generic;
using System.Linq;

namespace CellularAutomata.Automata
{
    public class ElementaryCa : CellularAutomata
    {
        //Elementary ca needs 2 rows, last row + current
        private const uint StateHeight = 2;

        //Number of possible inputs, sets required rule length
        private const uint InputCount = 8;

        //Position to store seed in, 1 stores seed in current row
        private const uint SeedStartRow = 1;

        private static readonly Dictionary<string, int[]> DefaultRules = new Dictionary<string, int[]>
        {
            {"Rule 102", new[]{0, 1, 1, 0, 0, 1, 1, 0}},
            {"Rule 110", new[]{0, 1, 1, 0, 1, 1, 1, 0}},
            {"Manual Rule", new[]{0} }
        };

        private static readonly string[] DefaultSeeds = {
            "Single Top Left",
            "Single Top Middle",
            "Single Top Right"
        };

        private static readonly Dictionary<string, int> DefaultDelays = new Dictionary<string, int>
        {
            {"Very Fast (25ms)", 25},
            {"Fast (50ms)", 50},
            {"Medium (100ms)", 100},
            {"Slow (150ms)", 150},
            {"Very Slow (200ms)", 200}
        };

        public ElementaryCa()
        {
            Setup(StateHeight, InputCount, SeedStartRow, GetRule(), GetSeed(), GetDelay());
        }

        //Rule must be an 8 digit binary number and seed must be a binary number that is shorter than current console width
        public ElementaryCa(int[] rule, int[,] seed, int delay)
        {
            Setup(StateHeight, InputCount, SeedStartRow, rule, seed, delay);
        }

        //Find next row by applying rule to previous row
        public override void Iterate()
        {
            base.Iterate();

            for (uint i = 1; i < State.GetLength(1) - 1; i++)
            {
                //Concatinate ints and convert to binary
                string total = State[0, i - 1].ToString() + State[0, i] + State[0, i + 1];
                int totalBinary = Convert.ToInt32(total, 2);

                //Set new cell to rule for totalBinary
                State[1, i] = Rule[totalBinary];
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

        private static int[] GetRule()
        {
            string option = UserRequest.GetOption("Select Rule", DefaultRules.Keys.ToArray(), true);
            int[] rule = DefaultRules[option];

            if (option == "Manual Rule")
            {
                //Reset rule
                rule = new int[8];
                //Remove rule answer
                Console.CursorTop = Console.CursorTop - 1;

                //TODO move to base and explain the 2
                //Get rule from user
                int number = UserRequest.GetNumber("Rule", (int)Math.Pow(2, InputCount), false);
                //Convert rule to 8 bit binary number
                string numberBinary = Convert.ToString(number, 2).PadLeft(8, '0');
                //Store rule in rule var in reverse order
                for (int i = 0; i < numberBinary.Length; i++)
                {
                    rule[InputCount - 1 - i] = int.Parse(numberBinary[i].ToString());
                }
            }

            return rule;
        }

        //TODO allow manual seed entry
        private static int[,] GetSeed()
        {
            string option = UserRequest.GetOption("Select Initial Row", DefaultSeeds, true);

            //For single top middle option to work seed length must be odd
            int maxSeedSize = Console.WindowWidth;
            if (maxSeedSize % 2 == 0)
            {
                maxSeedSize--;
            }

            int[,] seed = new int[1, maxSeedSize];

            switch (option)
            {
                case "Single Top Left":
                    seed[0, 0] = 1;
                    break;
                case "Single Top Middle":
                    seed[0, maxSeedSize / 2] = 1;
                    break;
                case "Single Top Right":
                    seed[0, maxSeedSize - 1] = 1;
                    break;
            }

            return seed;
        }

        private static int GetDelay()
        {
            string option = UserRequest.GetOption("Select Delay", DefaultDelays.Keys.ToArray(), true);

            return DefaultDelays[option];
        }
    }
}