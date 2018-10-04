using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace CellularAutomata.Automata
{
    public abstract class CellularAutomata
    {
        public static readonly int MaxSeedSize = Console.WindowWidth - 2 - (Console.WindowWidth - 1) % 2;

        protected uint TimesRan { get; set; }

        protected int Delay { get; set; }

        //Stores current state of the automata
        protected int[,] State { get; }

        //Rule determines the output for each input
        protected int[] Rule { get; }

        //Default colours used by CAs
        protected ConsoleColor[] Colours =
        {
            ConsoleColor.White,
            ConsoleColor.Black
        };

        //Whether or not the program can iterate
        protected bool Running { get; set; }

        private static readonly Dictionary<string, int> DefaultDelays = new Dictionary<string, int>
        {
            {"Very Fast (25ms)", 25},
            {"Fast (50ms)", 50},
            {"Medium (100ms)", 100},
            {"Slow (150ms)", 150},
            {"Very Slow (200ms)", 200}
        };

        //Height controls height of state array, input count is the number of inputs possible and controls rule length
        //and seed position controls y position to begin placing seed
        protected CellularAutomata(uint stateHeight, uint inputCount, Dictionary<string, int[]> allowedRules,
            int caBase, Dictionary<string, Point[]> allowedSeeds)
        {        
            Rule = GetRule(allowedRules, inputCount, caBase);
            State = GetSeed(allowedSeeds, stateHeight);
            Delay = GetDelay(DefaultDelays);
            Running = true;
        }

        //Height controls height of state array, input count is the number of inputs possible and controls rule length
        //and seed position controls y position to begin placing seed
        //#TODO limit max rule to prevent entering non existant rules
        protected CellularAutomata(uint stateHeight, uint inputCount, uint seedPosition, int[] rule, int[,] seed, int delay)
        {
            if (rule.Length != inputCount)
            {
                throw new ArgumentOutOfRangeException(nameof(rule), "Rule must contain exactly " + inputCount + " digits");
            }

            if (seed.GetLength(1) >= Console.WindowWidth)
            {
                throw new ArgumentOutOfRangeException(nameof(seed), "Seed can only be as long as the console's width");
            }

            Rule = rule;

            State = new int[stateHeight, seed.GetLength(1)];

            for (uint i = 0; i < seed.GetLength(0); i++)
            {
                for (uint j = 0; j < seed.GetLength(1); j++)
                {
                    State[seedPosition + i, j] = seed[i, j];
                }
            }

            Delay = delay;

            Running = true;
        }

        public virtual void Iterate()
        {
            if (!Running)
            {
                return;
            }
            //Shift 2d array back by 1 row to make room for new row
            State.ShiftBack();
            TimesRan++;

            //First bit cannot change
            State[State.GetLength(0) - 1, 0] = State[0, 0];

            //Last bit cannot change
            State[State.GetLength(0) - 1, State.GetLength(1) - 1] = State[0, State.GetLength(1) - 1];
        }

        public virtual void Draw()
        {
            Thread.Sleep(Delay);

            for (uint i = 0; i < State.GetLength(1); i++)
            {
                int previousCell = State[State.GetLength(0) - 2, i];
                int currentCell = State[State.GetLength(0) - 1, i];

                //Draws both cells using the bottom square unicode char
                //if both are the same then both colours are same and a rectangle is drawn even with the bottom square char
                Console.BackgroundColor = Colours[previousCell];
                Console.ForegroundColor = Colours[currentCell];
                Console.Write('▄');
            }

            Console.WriteLine();
        }

        //Setups up the console to draw cells, can optionally be overridden to change setup
        //TODO be able to change entire background colour after drawing has begun
        public virtual void SetupConsole()
        {
            Console.BackgroundColor = Colours[0];
            Console.CursorVisible = false;
            Console.Clear();
        }

        //Allows modification of CA once it has started, to e.g. change delay, reverse, colours
        //can optionally be overridden to add new modifications
        public virtual void Modify(Modification modification, params object[] arguments)
        {
            switch (modification)
            {
                case Modification.Colour:
                    if (arguments != null)
                    {
                        //Allows change colour at position
                        if (arguments.Length == 2 && arguments[0] is int position 
                                                  && arguments[1] is ConsoleColor positionColour)
                        {
                            Colours[position] = positionColour;
                        }
                        //Allows changing all colours
                        else if (arguments.Length == Colours.Length)
                        {
                            ConsoleColor[] backup = Colours;
                            for (int i = 0; i < arguments.Length; i++)
                            {
                                if (!(arguments[i] is ConsoleColor colour))
                                {
                                    Colours = backup;
                                    return;
                                }

                                Colours[i] = colour;
                            }
                        }
                    }
                    break;
                case Modification.Delay:
                    if (arguments != null && arguments.Length == 1 && arguments[0] is int delay && delay > 0)
                    {
                        Delay = delay;
                    }
                    break;
                case Modification.Running:
                    //Allows starting or stopping CA
                    if (arguments != null && arguments.Length == 1 && arguments[0] is bool run)
                    {
                        Running = run;
                    }
                    //Toggles CA
                    else
                    {
                        Running = !Running;
                    }
                    break;
            }
        }

        private static int[] GetRule(Dictionary<string, int[]> allowedRules, uint ruleLength, int caBase)
        {
            string option = UserRequest.GetOption("Select Rule", allowedRules.Keys.ToArray(), true);
            int[] rule = allowedRules[option];

            if (option == "Manual Rule")
            {
                //Reset rule
                rule = new int[8];
                //Remove rule answer
                Console.CursorTop = Console.CursorTop - 1;

                //Get rule from user
                int number = UserRequest.GetNumber("Rule", (int)Math.Pow(caBase, ruleLength), false);
                //Convert rule to 8 bit binary number
                string numberBinary = Convert.ToString(number, caBase).PadLeft(8, '0');
                //Store rule in rule var in reverse order
                for (int i = 0; i < numberBinary.Length; i++)
                {
                    rule[ruleLength - 1 - i] = int.Parse(numberBinary[i].ToString());
                }
            }

            return rule;
        }

        //TODO allow manual seed entry
        private static int[,] GetSeed(Dictionary<string, Point[]> allowedSeeds, uint stateHeight)
        {
            string option = UserRequest.GetOption("Select Initial Row", allowedSeeds.Keys.ToArray(), true);

            int[,] seed = new int[stateHeight, MaxSeedSize + 1];

            foreach (Point point in allowedSeeds[option])
            {
                if (point.X > MaxSeedSize)
                {
                    seed[point.Y, MaxSeedSize] = 1;
                }
                else
                {
                    seed[point.Y, point.X] = 1;
                }
                
            }

            return seed;
        }

        private static int GetDelay(Dictionary<string, int> allowedDelays)
        {
            string option = UserRequest.GetOption("Select Delay", allowedDelays.Keys.ToArray(), true);

            return allowedDelays[option];
        }
    }
}