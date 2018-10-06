using System;

namespace CellularAutomata.Device
{
    public class ConsoleAndKeyboard : Device
    {
        public override int CursorTop
        {
            get => Console.CursorTop;
            set => Console.CursorTop = value;
        }

        public override int CursorLeft
        {
            get => Console.CursorLeft;
            set => Console.CursorLeft = value;
        }

        public override bool CursorVisible
        {
            get => Console.CursorVisible;
            set => Console.CursorVisible = value;
        }

        public override void Clear()
        {
            Console.Clear();
        }

        public override ConsoleKeyInfo ReadKey(bool intercept)
        {
            return Console.ReadKey(intercept);
        }

        public override string ReadLine()
        {
            return Console.ReadLine();
        }

        public override void WriteLine(object value)
        {
            Console.WriteLine(value);
        }

        public override void Write(object value)
        {
            Console.Write(value);
        }

        public override string GetOption(string request, string[] options, bool resetClear)
        {
            while (true)
            {
                if (resetClear)
                {
                    Console.Clear();
                }

                Console.WriteLine(request + ":");

                for (int i = 1; i <= options.Length; i++)
                {
                    //Numbers are shifted up by one but options are not, this is to avoid displaying 0 as an option
                    Console.WriteLine(i + " : " + options[i - 1]);
                }

                //options 0 to options.length - 1 are displayed as 1 to options.length so 1 needs to be removed to line up with options
                if (int.TryParse(Console.ReadLine(), out int input) && input > 0 && input - 1 < options.Length)
                {
                    return options[input - 1];
                }
            }
        }

        public override int GetNumber(string request, int maxNumber, bool resetClear)
        {
            while (true)
            {
                if (resetClear)
                {
                    Console.Clear();
                }

                Console.Write(request + ": ");

                //Return if input is between 0 and maxNumber - 1, reask user otherwise
                if (int.TryParse(Console.ReadLine(), out int input) && input >= 0 && input < maxNumber)
                {
                    return input;
                }
            }
        }
    }
}