using System;

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
        private static readonly ConsoleColor[] DefaultColours = {
            //Used for cells with a value of 0
            ConsoleColor.White,
            //Used for cells with a value of 1
            ConsoleColor.Gray,
            //Used for cells with a value of 2
            ConsoleColor.Black
        };

        public ThreeColourTotalisticCa(int[] rule, int[,] seed, int delay)
        {
            Setup(StateHeight, InputCount, SeedStartRow, rule, seed, delay);
            Colours = DefaultColours;
        }

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
    }
}