using System;
using System.Threading;

namespace CellularAutomata.Automata
{
    public class ThreeColourTotalisticCa : CellularAutomata
    {
        //Elementary ca needs 2 rows, last row + current
        private const uint StateHeight = 2;

        //Number of possible inputs, sets required rule length, 3*colournum-2, i.e. 3*3-2 = 7 
        private const uint InputCount = 7;

        //Position to store seed in, Stores seed in current row
        private const uint SeedStartRow = 1;

        //Stores colours used for cells
        private static readonly ConsoleColor[] Colours = new ConsoleColor[3]
        {
            //Used for cells with a value of 0
            ConsoleColor.White,
            //Used for cells with a value of 1
            ConsoleColor.Gray,
            //Used for cells with a value of 2
            ConsoleColor.Black
        };

        public ThreeColourTotalisticCa(int[] rule, int[,] seed, int delay) 
            : base(StateHeight, InputCount, SeedStartRow, rule, seed, delay) {}

        //Find next row by applying rule to previous row
        public override void Iterate()
        {
            base.Iterate();

            for (uint i = 1; i < State.GetLength(1) - 1; i++)
            {
                //Add cells to find total for previous cells
                int total = State[0, i - 1] + State[0, i] + State[0, i + 1];

                //Set new cell to rule for total
                State[1, i] = Rule[total];
            }
        }

        //TODO Move back to base
        //Draws last row + current as squares by using the unicode block elements
        public override void Draw()
        {
            //Should only draw every second iteration, waits until second iteration as seed data only contains 1 row
            if (TimesRan % 2 == 0)
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
                Console.BackgroundColor = Colours[previousCell];
                Console.ForegroundColor = Colours[currentCell];
                Console.Write('▄');
            }

            Console.WriteLine();
        }
    }
}