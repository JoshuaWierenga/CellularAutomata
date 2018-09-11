using System;
using System.Collections;
using System.Threading;

namespace CellularAutomata
{
    public class SecondOrderReversibleCA : CellularAutomata
    {
        //Rule  determines the output for each 4 digit binary number where the least significant bit decides
        //the output for 0000 and the most significant bit decides the output for 1111
        private BitArray Rule { get; }

        //Rule must be a 16 digit binary number and seed must be a binary number that is shorter than max chars on console row
        public SecondOrderReversibleCA(BitArray rule, BitMatrix seedData, int delay)
        {
            if (rule.Length != 16)
            {
                throw new ArgumentOutOfRangeException(nameof(rule), "Rule must be an 16 digit binary number");
            }

            if (seedData.ColumnCount >= Console.WindowWidth)
            {
                throw new ArgumentOutOfRangeException(nameof(seedData), "Seed can only be as long as the console's width");
            }

            Rule = rule;
            //Reversible ca needs 3 rows, 2nd previous row, previous row and current
            //when next row is calculated, rows are pushed back by 1
            State = new BitMatrix(3, seedData.ColumnCount);
            for (uint i = 0; i < seedData.RowCount; i++)
            {
                for (uint j = 0; j < seedData.ColumnCount; j++)
                {
                    //Store first row in previous row and second row in current row
                    State[i + 1, j] = seedData[i, j];
                }
            }

            Delay = delay;
        }

        //Find next row by applying rule to previous row and 2nd previous row
        public override void Iterate()
        {
            //Shift 2d array back by 1 row to make room for new row
            State.ShiftBack();

            //First bit cannot change
            State[2, 0] = State[0, 0];

            for (uint i = 1; i < State.ColumnCount - 1; i++)
            {
                bool aboveBit = State[0, i];
                bool previousBit = State[1, i - 1];
                bool currentBit = State[1, i];
                bool nextBit = State[1, i + 1];

                //0000
                if (!aboveBit && !previousBit && !currentBit && !nextBit)
                {
                    State[2, i] = Rule[0];
                }
                //0001
                else if (!aboveBit && !previousBit && !currentBit)
                {
                    State[2, i] = Rule[1];
                }
                //0010
                else if (!aboveBit && !previousBit && !nextBit)
                {
                    State[2, i] = Rule[2];
                }
                //0011
                else if (!aboveBit && !previousBit)
                {
                    State[2, i] = Rule[3];
                }
                //0100
                else if (!aboveBit && !currentBit && !nextBit)
                {
                    State[2, i] = Rule[4];
                }
                //0101
                else if (!aboveBit && !currentBit)
                {
                    State[2, i] = Rule[5];
                }
                //0110
                else if (!aboveBit && !nextBit)
                {
                    State[2, i] = Rule[6];
                }
                //0111
                else if (!aboveBit)
                {
                    State[2, i] = Rule[7];
                }
                //1000
                else if (!previousBit && !currentBit && !nextBit)
                {
                    State[2, i] = Rule[8];
                }
                //1001
                else if (!previousBit && !currentBit)
                {
                    State[2, i] = Rule[9];
                }
                //1010
                else if (!previousBit && !nextBit)
                {
                    State[2, i] = Rule[10];
                }
                //1011
                else if (!previousBit)
                {
                    State[2, i] = Rule[11];
                }
                //1100
                else if (!currentBit && !nextBit)
                {
                    State[2, i] = Rule[12];
                }
                //1101
                else if (!currentBit)
                {
                    State[2, i] = Rule[13];
                }
                //1110
                else if (!nextBit)
                {
                    State[2, i] = Rule[14];
                }
                //1111
                else
                {
                    State[2, i] = Rule[15];
                }
            }

            //Last bit cannot change
            State[2, State.ColumnCount - 1] = State[0, State.ColumnCount - 1];

            TimesRan++;
        }

        public override void Draw()
        {
            //Should only draw every second iteration, runs on first iteration as seed data contains 2 rows
            if (TimesRan % 2 == 1)
            {
                return;
            }

            for (uint i = 0; i < State.ColumnCount; i++)
            {
                //Check if previous row is 1 in this position
                if (State[1, i])
                {
                    //If both the last row and this row are 1 at i then draw 2 stacked squares
                    //else if just the previous row then draw a square in the top half of the char
                    Console.Write(State[2, i] ? '█' : '▀');
                }
                else
                {
                    //If this row is 1 at i in the current row then draw a square in the bottom half of the char
                    //else leave it black and just draw a space
                    Console.Write(State[2, i] ? '▄' : ' ');
                }
            }

            Console.WriteLine();
            Thread.Sleep(Delay);
        }

        public override void SetupConsole()
        {
            //TODO allow changing cell colour
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.CursorVisible = false;
            Console.Clear();
        }

        //Allows modification of CA once it has starts, to i.e. change delay, reverse, change colours
        public override void Modify(string[] arguments)
        {
            for (int i = 0; i < arguments.Length; i++)
            {
                string option = arguments[i];

                switch (option)
                {
                    case "reverse":
                        Reverse();
                        break;
                    case "delay":
                        if (!int.TryParse(arguments[i + 1], out int delay) || delay < 0)
                        {
                            throw new ArgumentOutOfRangeException(nameof(delay), "Delay needs to be a positive integer");
                        }
                        i++;
                        Delay = delay;
                        break;
                    default:
                        throw new ArgumentException("Option is not valid", nameof(option));
                        
                }
            }
          
        }

        public void Reverse()
        {
            for (uint i = 0; i < State.ColumnCount; i++)
            {
                bool backup = State[2, i];
                State[2, i] = State[1, i];
                State[1, i] = backup;
            }

            TimesRan++;
            Iterate();
        }
    }
}