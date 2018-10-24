using CellularAutomata.Devices.BaseDevices;
using RaspberrySharp.Components.Inputs.COM14662;
using System;

namespace CellularAutomata.Devices.Inputs
{
    class PiCOM14662Keypad : Input
    {
        //Needs access to second display to be able to write input chars to it
        private readonly SecondaryDisplay _display;

        public sealed override bool ReducedInput { get; protected set; }

        //TODO allow changing pins
        public PiCOM14662Keypad(SecondaryDisplay textDisplay = null)
        {
            //Keypads only has 12 buttons so special setup is required to handle both numbers and arrows
            ReducedInput = true;
            _display = textDisplay;
        }

        //Handles getting a single character input from user
        //intercept controls whether to stop input being written the screen, input controls whether to get number or arrow entry
        public override ConsoleKeyInfo ReadKey(bool intercept, InputType input)
        {
            //Wait until an key allowed by input is pressed         
            while (true)
            {
                //Get char from user
                char key = COM14662.ReadChar();

                //Inputs that can always occur
                switch (key)
                {
                    //Backspace
                    case '*':
                        if (intercept || _display.CursorLeft <= 0)
                        {
                            return new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false);
                        }
                        //Remove last char
                        _display.CursorLeft--;
                        _display.Write(' ');
                        _display.CursorLeft--;
                        return new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false);
                    //Enter
                    case '#':
                        return new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false);
                }

                //Number and movement inputs
                switch (input)
                {
                    case InputType.Numbers when char.IsDigit(key):
                        if (!intercept)
                        {
                            _display.Write(key);
                        }
                        return new ConsoleKeyInfo(key, (ConsoleKey)Enum.Parse(typeof(ConsoleKey), "D" + key), false, false, false);
                    case InputType.Arrows:
                        switch (key)
                        {
                            //Left Arrow
                            case '4':
                                return new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false);
                            //Right Arrow
                            case '6':
                                return new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false);
                            //Up Arrow
                            case '2':
                                return new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false);
                            //Down Arrow
                            case '8':
                                return new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false);
                        }

                        break;
                }
            }
        }

        //Handles getting an entire string from the user
        public override string ReadLine()
        {
            string input = "";

            //Add each char to input until enter is pressed, then return input
            while (true)
            {
                //Get number from user
                ConsoleKeyInfo pressedKey = ReadKey(false, InputType.Numbers);

                switch (pressedKey.Key)
                {
                    case ConsoleKey.Backspace when input.Length != 0:
                        input = input.Remove(input.Length - 1);
                        break;
                    case ConsoleKey.Enter when input.Length != 0:
                        _display.CursorVisible = false;
                        return input;
                    default:
                        input += pressedKey.KeyChar;

                        //Clear current line if this is the first char
                        if (!_display.CursorVisible)
                        {
                            //Clear rest of the current line
                            _display.CursorLeft = 1;
                            _display.Write("".PadRight(_display.DisplaySize.Width - 1, ' '));
                            //Move cursor one space after first char
                            _display.CursorLeft = 1;
                            _display.CursorVisible = true;
                        }

                        break;
                }
            }
        }
    }
}