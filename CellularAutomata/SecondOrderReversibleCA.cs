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
                //Concatinate ints and convert to binary
                string total = State[0, i].ToString() + State[1, i - 1] + State[1, i] + State[1, i + 1];
                int totalBinary = Convert.ToInt32(total, 2);

                //Set new cell to rule for totalBinary
                State[2, i] = Rule[totalBinary];
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