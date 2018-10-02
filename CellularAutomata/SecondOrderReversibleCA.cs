using System;

namespace CellularAutomata
{
    public class SecondOrderReversibleCA : Automata.CellularAutomata
    {
        //Reversible ca needs 3 rows, 2nd previous row, previous row and current
        private const uint StateHeight = 3;

        //Number of possible inputs, sets required rule length
        private const uint InputCount = 16;

        //Position to start storing seed in, Stores seed in previous row + current row, 
        private const uint SeedPosition = 1;

        //Rule must be a 16 digit binary number and seed must be a binary number that is shorter than max chars on console row
        public SecondOrderReversibleCA(int[] rule, int[,] seed, int delay) 
            : base(StateHeight, InputCount, SeedPosition, rule, seed, delay) {}

        //Find next row by applying rule to previous row and 2nd previous row
        public override void Iterate()
        {
            base.Iterate();

            for (uint i = 1; i < State.GetLength(1) - 1; i++)
            {
                bool aboveBit = State[0, i] == 1;
                bool previousBit = State[1, i - 1] == 1;
                bool currentBit = State[1, i] == 1;
                bool nextBit = State[1, i + 1] == 1;

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
        }

        public override void Draw()
        {
            //Should only draw every second iteration, runs on first iteration as seed data contains 2 rows
            if (TimesRan % 2 == 1)
            {
                return;
            }

            base.Draw();   
        }

        //Allows modification of CA once it has started, to i.e. change delay, reverse, change colours
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
                }
            }
          
        }

        //Reverses CA by moving previous row after current row
        private void Reverse()
        {
            for (uint i = 0; i < State.GetLength(1); i++)
            {
                int backup = State[2, i];
                State[2, i] = State[1, i];
                State[1, i] = backup;
            }

            TimesRan++;
            Iterate();
        }
    }
}