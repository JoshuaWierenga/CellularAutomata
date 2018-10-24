using CellularAutomata.Devices.BaseDevices;
using CellularAutomata.Devices.Inputs;
using System;

namespace CellularAutomata.Devices
{
    //Support for consoles with a keybaord
    public class ConsoleAndKeyboard : Device
    {
        public ConsoleAndKeyboard()
        {
            Input = new Keyboard();
            SecondaryDisplay = null;
        }

        //Shows request and options to the user and returns selected option
        //location controls which screen the request and/or options are displayed on but is ignored since only output is the console
        //clear controls whether or not the clear before displaying request and/or options
        public override string GetOption(OutputLocation location, string request, string[] options, bool clear)
        {
            //Rerequest until option is picked
            while (true)
            {
                if (clear)
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
                if (int.TryParse(Input.ReadLine(), out int input) && input > 0 && input - 1 < options.Length)
                {
                    //Options 0 to options.length - 1 are displayed as 1 to options.length so 1 needs to be removed to line up with options
                    return options[input - 1];
                }
            }
        }
    
        //Gets number between 0 and maxNumber from user
        public override int GetNumber(string request, int maxNumber, bool clear)
        {
            //Rerequest until number between 0 and  maxNumber is entered
            while (true)
            {
                if (clear)
                {
                    Console.Clear();
                }

                //Indicate expected input
                Console.WriteLine("Input should be a 1 to " + Math.Ceiling(Math.Log10(maxNumber)) +
                                  " digit number between " + 0 + " and " + maxNumber);
                Console.Write(request + ": ");

                //Return if input is a number between 0 and option.Length - 1, restart otherwise
                if (int.TryParse(Input.ReadLine(), out int input) && input >= 0 && input <= maxNumber)
                {
                    return input;
                }
            }
        }
    }
}