using System;
using System.Collections;

namespace CellularAutomata
{
    public class ElementaryCA : CellularAutomata
    {
        //Rule  determines the output for each 3 digit binary number where the least significant bit decides
        //the output for 000 and the most significant bit decides the output for 111
        private BitArray Rule { get; }

        public ElementaryCA(BitArray rule, BitArray seedData)
        {
            Rule = rule;
            //Elementary ca only needs 2 rows, last row + current, when next row is calculated, rows are pushed back by 1
            State = new BitMatrix(2, (uint)seedData.Length);
            for (int i = 0; i < seedData.Count; i++)
            {
                //Stores seed in current row
                State[1, (uint)i] = seedData[i];
            }

        }

        //Find next row by applying rule to previous row
        public override void Iterate()
        {
            //Shift 2d array back by 1 row to make room for new row
            State.ShiftBack();

            //First bit cannot change
            State[1, 0] = State[0, 0];

            for (uint i = 1; i < State.ColumnCount - 1; i++)
            {
                bool previousBit = State[0, i - 1];
                bool currentBit = State[0, i];
                bool nextBit = State[0, i + 1];

                //000
                if (!previousBit && !currentBit && !nextBit)
                {
                    State[1, i] = Rule[0];
                }
                //001
                else if (!previousBit && !currentBit)
                {
                    State[1, i] = Rule[1];
                }
                //010
                else if (!previousBit && !nextBit)
                {
                    State[1, i] = Rule[2];
                }
                //011
                else if (!previousBit)
                {
                    State[1, i] = Rule[3];
                }
                //100
                else if (!currentBit && !nextBit)
                {
                    State[1, i] = Rule[4];
                }
                //101
                else if (!currentBit)
                {
                    State[1, i] = Rule[5];
                }
                //110
                else if (!nextBit)
                {
                    State[1, i] = Rule[6];
                }
                //111
                else
                {
                    State[1, i] = Rule[7];
                }

                //Last bit cannot change
                State[1, State.ColumnCount - 1] = State[0, State.ColumnCount - 1];
            }
        }

        //Draws last row + current as squares by using the unicode block elements
        public override void Draw()
        {
            for (uint i = 0; i < State.ColumnCount; i++)
            {
                //Check if previous row is 1 in this position
                if (State[0, i])
                {
                    //If both the last row and this row are 1 at i then draw 2 stacked squares
                    //else if just the previous row then draw a square in the top half of the char
                    Console.Write(State[1, i] ? '█' : '▀');
                }
                else
                {
                    //If this row is 1 at i then draw a square in the bottom half of the char
                    //else leave it black and just draw a space
                    Console.Write(State[1, i] ? '▄' : ' ');
                }
            }

            Console.WriteLine();
        }

        public override void SetupConsole()
        {
            //TODO allow changing cell colour
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.CursorVisible = false;
            Console.Clear();
        }
    }
}