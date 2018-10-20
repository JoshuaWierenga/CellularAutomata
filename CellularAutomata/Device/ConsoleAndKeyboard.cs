using System;

namespace CellularAutomata.Device
{
    //Support for consoles with a keybaord
    public class ConsoleAndKeyboard : Device
    {
        //Whether arrows and numbers are enterable at once or not,
        //set to false in constructor as keyboard has both
        public sealed override bool ReducedInput { get; protected set; }

        //Controls cursor on an external display, no such display is defined so these variables do nothing
        public override int CursorTop { get; set; }
        public override int CursorLeft { get; set; }
        public override bool CursorVisible { get; set; }

        //Clears an external display, no such display is defined so this function does nothing
        public override void Clear() {}

        //Writes value + newline to an external display, no such display is defined so this function does nothing
        public override void WriteLine(object value) {}

        //Writes value to an external display, no such display is defined so this function does nothing
        public override void Write(object value) {}

        public ConsoleAndKeyboard()
        {
            ReducedInput = false;
        }

        //Reads a char form some form of keyboard, in this case the console
        //intercept allows preventing key from appearing on display
        public override ConsoleKeyInfo ReadKey(bool intercept, InputType inputType)
        {
            //Can ignore inputType since keyboard contains both numbers and arrows
            return Console.ReadKey(intercept);
        }

        //Reads a string form some form of keyboard, in this case the console
        public override string ReadLine()
        {
            return Console.ReadLine();
        }

        //Display list of options to user and returns picked option
        public override string GetOption(string request, string[] options, bool resetClear)
        {
            //Rerequest until option is picked
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

                //Returns if input is a number between 1 and option.Length
                if (int.TryParse(Console.ReadLine(), out int input) && input > 0 && input - 1 < options.Length)
                {
                    //Options 0 to options.length - 1 are displayed as 1 to options.length so 1 needs to be removed to line up with options
                    return options[input - 1];
                }
            }
        }
    
        //Gets number between 0 and maxNumber from user
        public override int GetNumber(string request, int maxNumber, bool resetClear)
        {
            //Rerequest until number between 0 and  maxNumber is entered
            while (true)
            {
                if (resetClear)
                {
                    Console.Clear();
                }

                //Indicate expected input
                Console.WriteLine("Input should be a 1 to " + Math.Ceiling(Math.Log10(maxNumber)) +
                                  " digit number between " + 0 + " and " + maxNumber);
                Console.Write(request + ": ");

                //Return if input is between 0 and maxNumber, reask user otherwise
                if (int.TryParse(Console.ReadLine(), out int input) && input >= 0 && input <= maxNumber)
                {
                    return input;
                }
            }
        }
    }
}