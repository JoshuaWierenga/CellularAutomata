using CellularAutomata.Devices.BaseDevices;
using CellularAutomata.Devices.Displays;
using CellularAutomata.Devices.Inputs;
using RaspberrySharp.Components.Displays.Hd44780;
using System;

namespace CellularAutomata.Devices
{
    //Support for a Raspberry Pi with a 2 x 16 Hd44780 based LCD as a secondary screen and a 3x4 button keypad for input
    public class PiLcdAndKeypad : Device
    {
        public PiLcdAndKeypad(Hd44780Pins lcdPins)
        {
            SecondaryDisplay = new PiHd44780Lcd(lcdPins);
            Input = new PiCOM14662Keypad(SecondaryDisplay);
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

                //Lcd must be cleared, may write to wrong row otherwise
                SecondaryDisplay.Clear();

                //Write request to both console and lcd, also shows button mappings for keypad
                Console.WriteLine(request + ':');
                SecondaryDisplay.WriteLine(request);
                SecondaryDisplay.WriteLine("Back: * Enter: #");

                for (int i = 1; i <= options.Length; i++)
                {
                    //Numbers are shifted up by one but options are not, this is to avoid displaying 0 as an option
                    Console.WriteLine(i + " : " + options[i - 1]);
                }

                //Move cursor to second line
                SecondaryDisplay.CursorTop = 1;

                //Returns if input is a number between 1 and option.Length
                if (int.TryParse(Input.ReadLine(), out int input) && input > 0 && input - 1 < options.Length)
                {
                    //Options 0 to options.length - 1 are displayed as 1 to options.Length so 1 needs to be removed to line up with options
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

                //Lcd must be cleared, may write to wrong row otherwise
                SecondaryDisplay.Clear();

                //Indicate expected input
                Console.WriteLine("Input should be a 1 to " + Math.Ceiling(Math.Log10(maxNumber)) +
                                  " digit number between " + 0 + " and " + maxNumber);

                //Write request to both console and lcd, also shows button mappings for keypad
                Console.WriteLine(request);
                SecondaryDisplay.WriteLine(request);
                SecondaryDisplay.WriteLine("Back: * Enter: #");

                //Move cursor to second line
                if (SecondaryDisplay != null) SecondaryDisplay.CursorTop = 1;

                //Return if input is between 0 and maxNumber, reask user otherwise
                if (int.TryParse(Input.ReadLine(), out int input) && input >= 0 && input <= maxNumber)
                {
                    return input;
                }
            }
        }
    }
}