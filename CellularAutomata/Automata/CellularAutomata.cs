using System;
using System.Threading;

namespace CellularAutomata.Automata
{
    public abstract class CellularAutomata
    {
        protected uint TimesRan { get; set; }

        protected int Delay { get; set; }

        //Stores current state of the automata
        protected int[,] State { get; }

        //Rule determines the output for each input
        protected int[] Rule { get; }

        //Height controls height of state array, input count is the number of inputs possible and controls rule length
        //and seed position controls y position to begin placing seed
        protected CellularAutomata(uint stateHeight, uint inputCount, uint seedPosition, int[] rule, int[,] seed, int delay)
        {
            if (rule.Length != inputCount)
            {
                throw new ArgumentOutOfRangeException(nameof(rule), "Rule must contain exactly" + inputCount + " digits");
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
        }

        public virtual void Iterate()
        {
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
                //Check if previous row is 1 in this position
                if (State[State.GetLength(0) - 2, i] == 1)
                {
                    //If both the last row and this row are 1 at i then draw 2 stacked squares
                    //else if just the previous row then draw a square in the top half of the char
                    Console.Write(State[State.GetLength(0) - 1, i] == 1 ? '█' : '▀');
                }
                else
                {
                    //If this row is 1 at i in the current row then draw a square in the bottom half of the char
                    //else leave it black and just draw a space
                    Console.Write(State[State.GetLength(0) - 1, i] == 1 ? '▄' : ' ');
                }
            }

            Console.WriteLine();
        }

        //Setups up the console to draw cells, can optionally be overridden to change setup
        public virtual void SetupConsole()
        {
            //TODO allow changing cell colour
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.CursorVisible = false;
            Console.Clear();
        }

        //Allows modification of CA once it has started, to i.e. change delay, reverse, colours
        //TODO allow changing cell colour
        public virtual void Modify(string[] arguments) { }
    }
}