using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace CellularAutomata.Automata
{
    public abstract class CellularAutomata
    {
        //Stores maximum CA size, sometimes 1 less than maximum size to ensure it is an odd number so that there is a middle
        public static readonly int MaxSeedSize = Console.WindowWidth - 2 - (Console.WindowWidth - 1) % 2;

        protected uint TimesRan { get; set; }

        protected int Delay { get; set; }

        //Stores current state of the automata
        protected int[,] State { get; }

        //Rule determines the output for each input
        protected int[] Rule { get; }

        //Default colours used by CAs
        protected ConsoleColor[] Colours { get; set; }

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

        private readonly Device.Device _device;

        //Height controls height of state array, input count is the number of inputs possible and controls rule length
        //and seed position controls y position to begin placing seed
        protected CellularAutomata(uint stateHeight, int inputCount, uint seedPosition, Device.Device device,
            Dictionary<string, int[]> allowedRules, int caBase, Dictionary<string, Dictionary<Point, int>> allowedSeeds,
            ConsoleColor[] colours)
        {
            Colours = colours;
            _device = device;
            Rule = GetRule(allowedRules, inputCount, caBase);
            Delay = GetDelay(DefaultDelays);
            State = GetSeed(allowedSeeds, caBase, stateHeight, seedPosition);
            Running = true;
        }

        //Height controls height of state array, input count is the number of inputs possible and controls rule length
        //and seed position controls y position to begin placing seed
        //#TODO limit max rule to prevent entering non existant rules
        protected CellularAutomata(uint stateHeight, uint inputCount, uint seedPosition, Device.Device device,
            int[] rule, int[,] seed, int delay, ConsoleColor[] colours)
        {        
            if (rule.Length != inputCount)
            {
                throw new ArgumentOutOfRangeException(nameof(rule), "Rule must contain exactly " + inputCount + " digits");
            }

            if (seed.GetLength(1) >= Console.WindowWidth)
            {
                throw new ArgumentOutOfRangeException(nameof(seed), "Seed can only be as long as the console's width");
            }

            Colours = colours;
            _device = device;
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

        //Find next row by applying rule to previous row(s)
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

        //Draws last row + current as squares by using the unicode block elements
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

        private int[] GetRule(Dictionary<string, int[]> allowedRules, int ruleLength, int caBase)
        {
            string option = _device.GetOption("Select Rule", allowedRules.Keys.ToArray(), true);
            int[] rule = allowedRules[option];

            if (option != "Manual Rule") return rule;

            //Reset rule
            rule = new int[ruleLength];
            //Remove rule answer
            _device.CursorTop--;

            int maxNumber = (int)Math.Pow(caBase, ruleLength) - 1;

            //Get rule from user
            //TODO shorten string
            _device.WriteLine("Rule should be entered as a 1 to " + Math.Ceiling(Math.Log10(maxNumber)) + " digit number between " + 0 + " and " + maxNumber);
            int numberDecimal = _device.GetNumber("Rule", maxNumber + 1, false);
            //Convert rule to a number as long as ruleLength in base caBase
            string numberBase = IntExtensions.BaseChange(numberDecimal, caBase).PadLeft(ruleLength, '0');
            //Store rule in rule var in reverse order
            for (int i = 0; i < numberBase.Length; i++)
            {
                rule[ruleLength - 1 - i] = int.Parse(numberBase[i].ToString());
            }

            return rule;
        }

        private int[,] GetSeed(Dictionary<string, Dictionary<Point, int>> allowedSeeds, int maxSeedValue, uint stateHeight, uint seedStart)
        {
            //Get seed from user
            //TODO change request to make sense for reversible CA
            string option = _device.GetOption("Select Initial Row", allowedSeeds.Keys.ToArray(), true);

            //Create array for seed, width has + 1 as width should be 0 to MaxSeedSize
            int[,] seed = new int[stateHeight, MaxSeedSize + 1];

            if (option == "Manual Seed")
            {
                //TODO tell user how seed should be entered
                //Clear console and enable cursor
                SetupConsole();
                Console.CursorVisible = true;

                while (Console.CursorVisible)
                {
                   ConsoleKeyInfo pressedKey = _device.ReadKey(true);

                    //Move cursor or end while loop
                    switch (pressedKey.Key)
                    {
                        case ConsoleKey.UpArrow when Console.CursorTop > 0:
                            Console.CursorTop--;
                            break;
                        case ConsoleKey.DownArrow when Console.CursorTop < stateHeight - seedStart - 1:
                            Console.CursorTop++;
                            break;
                        case ConsoleKey.LeftArrow when Console.CursorLeft > 0:
                            Console.CursorLeft--;
                            break;
                        case ConsoleKey.RightArrow when Console.CursorLeft < MaxSeedSize:
                            Console.CursorLeft++;
                            break;
                        case ConsoleKey.Enter:
                            Console.CursorVisible = false;
                            break;
                    }

                    //check if pressed char is a number
                    int pressedNumber = pressedKey.KeyChar - 48;
                    if (pressedNumber < 0 || pressedNumber >= maxSeedValue) continue;

                    //Keep backup of cursor position
                    int cursorX = Console.CursorLeft;
                    int cursorY = Console.CursorTop;
                    //Update seed
                    seed[seedStart + cursorY, cursorX] = pressedNumber;
                    //Update console and put cursor one right of last position
                    Console.BackgroundColor = Colours[pressedNumber];
                    Console.WriteLine(' ');
                    Console.CursorTop = cursorY;
                    Console.CursorLeft = cursorX + 1;
                }
            }
            else
            {
                foreach (KeyValuePair<Point, int> point in allowedSeeds[option])
                {
                    //Stores the int value in seed[y, x] unless x > console width then just use console width
                    if (point.Key.X > MaxSeedSize)
                    {
                        seed[point.Key.Y, MaxSeedSize] = point.Value;
                    }
                    else
                    {
                        seed[point.Key.Y, point.Key.X] = point.Value;
                    }
                }
            }

            return seed;
        }

        private int GetDelay(Dictionary<string, int> allowedDelays)
        {
            string option = _device.GetOption("Select Delay", allowedDelays.Keys.ToArray(), true);

            return allowedDelays[option];
        }
    }
}