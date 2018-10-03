using System;

namespace CellularAutomata
{
    public static class UserRequest
    {
        public static string GetOption(string request, string[] options, bool resetClear)
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

        public static int GetNumber(string request, int maxNumber, bool resetClear)
        {
            while (true)
            {
                if (resetClear)
                {
                    Console.Clear();
                }

                Console.Write(request + ": ");

                if (int.TryParse(Console.ReadLine(), out int input) && input >= 0 && input < maxNumber)
                {
                    return input;
                }
            }
        }
    }
}