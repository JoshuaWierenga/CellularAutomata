using System;
using System.Collections;
using System.Threading;

namespace CellularAutomata
{
    public abstract class CellularAutomata
    {
        protected uint TimesRan { get; set; }

        protected int Delay { get; set; }

        protected BitMatrix State { get; }

        //Rule determines the output for each input
        protected BitArray Rule { get; }

        //Height controls vertical size of state array, input count is the number of inputs possible and controls rule length
        //and seed position controls y position to begin placing seed
        protected CellularAutomata(uint caHeight, uint  inputCount, uint seedPosition, BitArray rule, BitMatrix seed, int delay)
        {
            if (rule.Length != inputCount)
            {
                throw new ArgumentOutOfRangeException(nameof(rule), "Rule must be an " + inputCount + " digit binary number");
            }

            if (seed.ColumnCount >= Console.WindowWidth)
            {
                throw new ArgumentOutOfRangeException(nameof(seed), "Seed can only be as long as the console's width");
            }

            Rule = rule;

            State = new BitMatrix(caHeight, seed.ColumnCount);

            for (uint i = 0; i < seed.RowCount; i++)
            {
                for (uint j = 0; j < seed.ColumnCount; j++)
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
            State[State.RowCount - 1, 0] = State[0, 0];

            //Last bit cannot change
            State[State.RowCount - 1, State.ColumnCount - 1] = State[0, State.ColumnCount - 1];
        }

        public virtual void Draw()
        {
            Thread.Sleep(Delay);

            for (uint i = 0; i < State.ColumnCount; i++)
            {
                //Check if previous row is 1 in this position
                if (State[State.RowCount - 2, i])
                {
                    //If both the last row and this row are 1 at i then draw 2 stacked squares
                    //else if just the previous row then draw a square in the top half of the char
                    Console.Write(State[State.RowCount - 1, i] ? '█' : '▀');
                }
                else
                {
                    //If this row is 1 at i in the current row then draw a square in the bottom half of the char
                    //else leave it black and just draw a space
                    Console.Write(State[State.RowCount - 1, i] ? '▄' : ' ');
                }
            }

            Console.WriteLine();
        }

        public virtual void SetupConsole()
        {
            //TODO allow changing cell colour
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.CursorVisible = false;
            Console.Clear();
        }

        //Allows modification of CA once it has started, to i.e. change delay, reverse, colours
        public abstract void Modify(string[] arguments);
    }
}