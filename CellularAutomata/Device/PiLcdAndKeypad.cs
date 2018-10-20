using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using RaspberrySharp.Components.Displays.Hd44780;
using RaspberrySharp.Components.Expanders.Pcf8574;
using RaspberrySharp.Components.Inputs.COM14662;
using RaspberrySharp.IO;
using RaspberrySharp.IO.GeneralPurpose;
using RaspberrySharp.IO.InterIntegratedCircuit;

namespace CellularAutomata.Device
{
    //Support for a Raspberry Pi with a 2 x 16 Hd44780 based LCD as a secondary screen and a 3x4 button keypad for input
    public class PiLcdAndKeypad : Device
    {
        //Handles communication to lcd
        private readonly Hd44780LcdConnection _lcd;
        private Hd44780Position _cursorPosition;
        private readonly Size _lcdSize = new Size(16, 2);

        //Whether arrows and numbers are enterable at once or not,
        //set to true in constructor as 3x4 keypad
        public sealed override bool ReducedInput { get; protected set; }

        //Gets or sets cursor vertical postion, setting position updates the display
        public override int CursorTop
        {
            get => _cursorPosition.Row;
            set
            {
                //Wrap if less than 0
                if (value < 0)
                {
                    _cursorPosition.Row = _lcdSize.Height - 1;
                }
                //Wrap if more than 1
                else if (value >= _lcdSize.Height)
                {
                    _cursorPosition.Row = 0;
                }
                //Otherwise set
                else
                {
                    _cursorPosition.Row = value;
                }
                _lcd.SetCursorPosition(_cursorPosition);
            }
        }

        //Gets or sets cursor horizontal postion, setting position updates the display
        public override int CursorLeft
        {
            get => _cursorPosition.Column;
            set
            {
                //Wrap if less than 0
                if (value < 0)
                {
                    _cursorPosition.Column = _lcdSize.Width - 1;
                }
                //Wrap if more than 15
                else if (value >= _lcdSize.Width)
                {
                    _cursorPosition.Column = 0;
                }
                //Otherwise set
                else
                {
                    _cursorPosition.Column = value;
                }
                _lcd.SetCursorPosition(_cursorPosition);
            }
        }

        public override bool CursorVisible
        {
            get => _lcd.CursorEnabled;
            set => _lcd.CursorEnabled = value;
        }

        //Connects to the Hd44780
        public PiLcdAndKeypad(byte i2CAddress, Hd44780Pins pins)
        {
            ReducedInput = true;

            Hd44780LcdConnectionSettings settings = new Hd44780LcdConnectionSettings {ScreenWidth = _lcdSize.Width};
            _lcd = new Hd44780LcdConnection(settings, pins);
        }

        //Clears an external display
        public override void Clear()
        {
            _lcd.Clear();
        }

        //Reads a char form some form of keyboard, in this case a 12 char keypad
        //intercept allows preventing key from appearing on display
        public override ConsoleKeyInfo ReadKey(bool intercept, InputType input)
        {
            //Wait until an key allowed by input is pressed         
            while (true)
            {
                char key = COM14662.ReadChar();

                //Inputs that can always occur
                switch (key)
                {
                    //Backspace
                    case '*':
                        if (intercept || CursorLeft <= 0)
                        {
                            return new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false);
                        }
                        //Remove last char
                        CursorLeft--;
                        Write(' ');
                        CursorLeft--;
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
                            Write(key);
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

        //Reads a string form some form of keyboard, in this case a 12 char keypad
        public override string ReadLine()
        {
            string input = "";

            while (true)
            {
                ConsoleKeyInfo pressedKey = ReadKey(false, InputType.Numbers);
                switch (pressedKey.Key)
                {
                    case ConsoleKey.Backspace when input.Length != 0:
                        input = input.Remove(input.Length - 1);
                        break;
                    case ConsoleKey.Enter when input.Length != 0:
                        CursorVisible = false;
                        return input;
                    default:
                        input += pressedKey.KeyChar;

                        if (!CursorVisible)
                        {
                            //Clear rest of the current line
                            CursorLeft = 1;
                            Write("".PadRight(_lcdSize.Width - 1, ' '));
                            //Move cursor one space after first char
                            CursorLeft = 1;
                            CursorVisible = true;
                        }

                        break;
                }
            }
        }

        //Writes value to an external display
        public override void Write(object value)
        {
            _lcd.Write(value);
            _cursorPosition.Column++;
        }

        //Writes value + newline to an external display
        public override void WriteLine(object value)
        {
            _lcd.WriteLine(value);
            _cursorPosition.Column = 0;
            _cursorPosition.Row++;
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
                Clear();

                //Write request to both console and lcd, also shows button mappings for keypad
                Console.WriteLine(request + ':');
                WriteLine(request);
                WriteLine("Back: * Enter: #");

                for (int i = 1; i <= options.Length; i++)
                {
                    //Numbers are shifted up by one but options are not, this is to avoid displaying 0 as an option
                    Console.WriteLine(i + " : " + options[i - 1]);
                }

                //Move cursor to second line
                CursorTop = 1;

                //Returns if input is a number between 1 and option.Length
                if (int.TryParse(ReadLine(), out int input) && input > 0 && input - 1 < options.Length)
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
                Clear();

                //Indicate expected input
                Console.WriteLine("Input should be a 1 to " + Math.Ceiling(Math.Log10(maxNumber)) +
                                  " digit number between " + 0 + " and " + maxNumber);

                //Write request to both console and lcd, also shows button mappings for keypad
                Console.WriteLine(request);
                WriteLine(request);
                WriteLine("Back: * Enter: #");

                //Move cursor to second line
                CursorTop = 1;

                //Return if input is between 0 and maxNumber, reask user otherwise
                if (int.TryParse(ReadLine(), out int input) && input >= 0 && input <= maxNumber)
                {
                    return input;
                }
            }
        }

        //Setup Hd44780 connection when connected directly to the Raspberry Pi
        private static Hd44780Pins LoadGpioConfiguration(ProcessorPin registerSelectPin, ProcessorPin clockPin, IEnumerable<ProcessorPin> dataPins)
        {
            IGpioConnectionDriver driver = GpioConnectionSettings.DefaultDriver;
            return new Hd44780Pins(
                driver.Out(registerSelectPin),
                driver.Out(clockPin),
                dataPins.Select(p => (IOutputBinaryPin) driver.Out(p)));
        }

        //Setup Hd44780 connection when connected to the Raspberry Pi though a Pcf8574 I2c io expander
        public static Hd44780Pins SetupPcf8574Connection(int address)
        {
            const Pcf8574Pin clockPin = Pcf8574Pin.P2;
            const Pcf8574Pin readWritePin = Pcf8574Pin.P1;
            const Pcf8574Pin registerSelectPin = Pcf8574Pin.P0;
            const Pcf8574Pin backlightPin = Pcf8574Pin.P3;
            Pcf8574Pin[] dataPins = {
                Pcf8574Pin.P4,
                Pcf8574Pin.P5,
                Pcf8574Pin.P6,
                Pcf8574Pin.P7
            };

            const ProcessorPin sdaPin = ProcessorPin.Gpio02;
            const ProcessorPin sclPin = ProcessorPin.Gpio03;

            I2cDriver driver = new I2cDriver(sdaPin, sclPin) { ClockDivider = 512 };
            Pcf8574I2CConnection connection = new Pcf8574I2CConnection(driver.Connect(address));
            return new Hd44780Pins(
                connection.Out(registerSelectPin),
                connection.Out(clockPin),
                dataPins.Select(p => (IOutputBinaryPin)connection.Out(p)))
            {
                Backlight = connection.Out(backlightPin),
                ReadWrite = connection.Out(readWritePin),
            };
        }
    }
}