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
        public override string GetOption(OutputLocation location, string request, string[] options, bool clear)
        {
            //Rerequest until option is picked
            while (true)
            {

                //Console should only be cleared if it us being used and it has been requested
                if (clear && location != OutputLocation.Secondary)
                {
                    Console.Clear();
                }

                if (location != OutputLocation.Console)
                {
                    //Lcd must be cleared if being used, may write to wrong row otherwise
                    SecondaryDisplay.Clear();

                    SecondaryDisplay.WriteLine(request);
                    SecondaryDisplay.Write("Back: * ");
                }

                int selectedOption;

                //Write request + options to console and write request to secondary display if using it
                if (location != OutputLocation.Secondary)
                {
                    //Write request to console and finish secondary display setup
                    Console.WriteLine(request + ':');
                    if (location != OutputLocation.Console)
                    {
                        SecondaryDisplay.Write("Enter: #");
                        SecondaryDisplay.CursorLeft = 0;
                    }                    

                    //Write options to console
                    for (int i = 1; i <= options.Length; i++)
                    {
                        //Numbers are shifted up by one but options are not, this is to avoid displaying 0 as an option
                        Console.WriteLine(i + " : " + options[i - 1]);
                    }

                    //Options 0 to options.length - 1 are displayed as 1 to options.Length so 1 needs to be removed to line up with options
                    selectedOption = int.Parse(Input.ReadLine()) - 1;
                }
                else
                {
                    selectedOption = 0;

                    SecondaryDisplay.Write("Begin: #");

                    //Get number from user
                    ConsoleKeyInfo inputChar = Input.ReadKey(true, InputType.Numbers);

                    //only continue if char is enter
                    while (inputChar.Key != ConsoleKey.Enter)
                    {
                       inputChar = Input.ReadKey(true, InputType.Numbers);
                    }

                    bool selecting = true;

                    //Show options to user until selection has been made
                    while (selecting)
                    {
                        //Clear line and write option
                        SecondaryDisplay.CursorLeft = 0;
                        SecondaryDisplay.Write(options[selectedOption].PadRight(SecondaryDisplay.DisplaySize.Width));

                        //Get direction for user
                        inputChar = Input.ReadKey(true, InputType.Arrows);

                        //Move between options if left or right arrow was pressed, stop and continue fuction if enter was pressed
                        switch (inputChar.Key)
                        {
                            case ConsoleKey.LeftArrow when selectedOption > 0:
                                selectedOption--;
                                break;
                            case ConsoleKey.RightArrow when selectedOption < options.Length - 1:
                                selectedOption++;
                                break;
                            case ConsoleKey.Enter:
                                selecting = false;
                                break;
                        }
                    }
                }

                //Return if input is a number between 0 and option.Length - 1, restart otherwise
                if (selectedOption >= 0 && selectedOption < options.Length)
                {               
                    return options[selectedOption];
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
                SecondaryDisplay.CursorTop = 1;

                //Return if input is between 0 and maxNumber, reask user otherwise
                if (int.TryParse(Input.ReadLine(), out int input) && input >= 0 && input <= maxNumber)
                {
                    return input;
                }
            }
        }
    }
}