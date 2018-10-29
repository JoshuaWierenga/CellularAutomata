using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using CellularAutomata.Devices.BaseDevices;

namespace CellularAutomata.Automata
{
    public abstract class CellularAutomata
    {
        //Stores maximum CA size, sometimes 1 less than maximum size to ensure it is an odd number so that there is a middle
        public static readonly int MaxSeedSize = Console.WindowWidth - 2 - (Console.WindowWidth - 1) % 2;

        //Number of times the automata has iterated
        protected uint TimesRan { get; set; }

        //Delay between drawing each row
        protected int Delay { get; set; }

        //Stores current state of the automata
        protected int[,] State { get; }

        //Rule determines the output for each input
        protected int[] Rule { get; }

        //Colours used by CAs
        public virtual ConsoleColor[] Colours => AutomataDefaults.DefaultColours;

        //Whether or not the program can iterate
        public bool Running = true;

        //Defaults delays between ca draws, more can be added
        public virtual Dictionary<string, int> Delays => AutomataDefaults.DefaultDelays;

        //Handles getting user input and writing to displays other than the console
        private readonly Device _device;

        public virtual Dictionary<string, (Type modification, Dictionary<string, Type> arguments)> Modifications => AutomataDefaults.DefaultModifications;

        //Height controls height of state array, input count is the number of inputs possible and is the max rule length
        //seed position controls y position to begin placing seed, device allows control of hardware other than the console
        //and caBase is the numerical base used for ca iteration
        protected CellularAutomata(uint stateHeight, int inputCount, uint seedPosition, Device device,
            Dictionary<string, int[]> allowedRules, int caBase, Dictionary<string, Dictionary<Point, int>> allowedSeeds)
        {
            _device = device;
            Rule = GetRule(allowedRules, inputCount, caBase);
            Delay = GetDelay();
            State = GetSeed(allowedSeeds, caBase, stateHeight, seedPosition);
        }

        //Height controls height of state array, input count is the number of inputs possible and is the max rule length
        //seed position controls y position to begin placing seed and device allows control of hardware other than the console
        //#TODO limit max rule to prevent entering non existant rules
        protected CellularAutomata(uint stateHeight, uint inputCount, uint seedPosition, Device device,
            int[] rule, int[,] seed, int delay)
        {        
            if (rule.Length != inputCount)
            {
                throw new ArgumentOutOfRangeException(nameof(rule), "Rule must contain exactly " + inputCount + " digits");
            }

            if (seed.GetLength(1) != MaxSeedSize)
            {
                throw new ArgumentOutOfRangeException(nameof(seed), "Seed must contain exactly " + MaxSeedSize + " digits");
            }

            _device = device;
            Rule = rule;
            State = new int[stateHeight, seed.GetLength(1)];

            //Move seed into state starting at row seedPosition
            for (uint i = 0; i < seed.GetLength(0); i++)
            {
                for (uint j = 0; j < seed.GetLength(1); j++)
                {
                    State[seedPosition + i, j] = seed[i, j];
                }
            }

            Delay = delay;
        }

        //Find next row by applying rule to previous row(s)
        //this function on moves the state back and sets the outer cells, full iteration occurs in CAs
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
        //TODO draw entire row in one go to improve speed
        public virtual void Draw()
        {
            if (!Running)
            {
                return;
            }

            Thread.Sleep(Delay);

            for (uint i = 0; i < State.GetLength(1); i++)
            {
                int previousCell = State[State.GetLength(0) - 2, i];
                int currentCell = State[State.GetLength(0) - 1, i];

                //Draws both cells using the bottom square unicode char
                //if both are the same then both colours are same and a rectangle is drawn even with the bottom square char
                //otherwise the colours will be different and it will look like two squares
                Console.BackgroundColor = Colours[previousCell];
                Console.ForegroundColor = Colours[currentCell];
                Console.Write('▄');
            }

            Console.WriteLine();
        }

        //Setups up the console to draw cells, can optionally be overridden to change setup
        //TODO fix backgroundColor on raspberry pi returning to black once console begins scrolling
        //TODO be able to change entire background colour after drawing has begun
        public virtual void SetupConsole()
        {
            Console.BackgroundColor = Colours[0];
            Console.CursorVisible = false;
            Console.Clear();
            _device.SecondaryDisplay?.Clear();
        }

        //Gets rule from the user, allows both the selection of a rule from allowedRules or manual entry using ruleLength and caBase to define allowed rules
        private int[] GetRule(Dictionary<string, int[]> allowedRules, int ruleLength, int caBase)
        {
            //Get rule from the user
            string option = _device.GetOption(OutputLocation.Both, "Select Rule", allowedRules.Keys.ToArray(), true);

            if (option != "Manual Rule") return allowedRules[option];

            int[] rule = new int[ruleLength];

            //Get rule from user, caBase to the power of ruleLength - 1 is the maximum rule enterable
            int ruleDecimal = _device.GetNumber("Enter Rule", (int)Math.Pow(caBase, ruleLength) - 1, false);
            //Converts the rule to a number in caBase, padded to ruleLength digits, this is as 0's are a valid rule value
            string ruleBase = IntExtensions.BaseChange(ruleDecimal, caBase).PadLeft(ruleLength, '0');
            //Store rule in rule var in reverse order, this is so that the least significant rule bit will be subrule 0
            //and that most significant rule bit will be subrule ruleLength - 1
            for (int i = 0; i < ruleBase.Length; i++)
            {
                rule[ruleLength - 1 - i] = int.Parse(ruleBase[i].ToString());
            }

            return rule;
        }

        //Gets draw delay from the user
        //TODO allow custom delay length
        private int GetDelay()
        {
            //Get delay length from the user
            string option = _device.GetOption(OutputLocation.Both, "Select Delay", Delays.Keys.ToArray(), true);

            return Delays[option];
        }

        //Gets seed data from the user, allows both the selection of a rule from allowedSeeds or manual entry using maxSeedValue and stateHeight to define seed size
        private int[,] GetSeed(Dictionary<string, Dictionary<Point, int>> allowedSeeds, int maxSeedValue, uint stateHeight, uint seedStart)
        {
            //Get seed from the user
            string option = _device.GetOption(OutputLocation.Both, "Select Seed Data", allowedSeeds.Keys.ToArray(), true);

            //Create array for seed, width has + 1 as width should be 0 to MaxSeedSize
            int[,] seed = new int[stateHeight, MaxSeedSize + 1];

            if (option == "Manual Seed")
            {
                //TODO tell user how seed should be entered
                //Clear console and enable cursor
                SetupConsole();
                Console.CursorVisible = true;

                //Whether or not next input is cell selection or cell value
                //if input device supports both then this is ignored
                bool cellSelection = true;

                while (Console.CursorVisible)
                {
                    //Requests movement input unless cellSelection is false, then requests number input instead
                    ConsoleKeyInfo pressedKey = _device.Input.ReadKey(true, cellSelection ? InputType.Arrows : InputType.Numbers);

                    //Move cursor or end seed entry
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
                            //Hides the cursor and ends the while loop
                            Console.CursorVisible = false;
                            break;
                        case ConsoleKey.Backspace:
                            //Switches to allow cell value entry
                            cellSelection = false;
                            break;
                        default:
                            //If this input should affect cell values rather than selected cell
                            //when using input devices with more buttons like a keyboard this is always used
                            //otherwise only when last input switched to number input
                            if (!cellSelection || !_device.Input.ReducedInput)
                            {
                                //check if pressed char is a number and a valid seed value
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
                                cellSelection = true;
                            }
                            
                            break;
                    }                  
                }
            }
            else
            {
                //Copy seed values from allowedSeeds[option] to seed array
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
    }
}