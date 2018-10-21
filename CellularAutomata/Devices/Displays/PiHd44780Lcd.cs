using CellularAutomata.Devices.BaseDevices;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using RaspberrySharp.Components.Displays.Hd44780;
using RaspberrySharp.Components.Expanders.Pcf8574;
using RaspberrySharp.IO;
using RaspberrySharp.IO.GeneralPurpose;
using RaspberrySharp.IO.InterIntegratedCircuit;

namespace CellularAutomata.Devices.Displays
{
    //Handles communication to an Hd44780 from a Raspberry Pi
    public class PiHd44780Lcd : SecondaryDisplay
    {
        //Handles communication to lcd
        private readonly Hd44780LcdConnection _lcd;
        private Hd44780Position _cursorPosition;
        public sealed override Size DisplaySize { get; protected set; }

        //Gets or sets cursor vertical postion, setting position updates the display
        public override int CursorTop
        {
            get => _cursorPosition.Row;
            set
            {
                //Wrap if less than 0
                if (value < 0)
                {
                    _cursorPosition.Row = DisplaySize.Height - 1;
                }
                //Wrap if more than 1
                else if (value >= DisplaySize.Height)
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
                    _cursorPosition.Column = DisplaySize.Width - 1;
                }
                //Wrap if more than 15
                else if (value >= DisplaySize.Width)
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
        public PiHd44780Lcd(Hd44780Pins pins)
        {
            DisplaySize = new Size(16, 2);

            Hd44780LcdConnectionSettings settings = new Hd44780LcdConnectionSettings { ScreenWidth = DisplaySize.Width };
            _lcd = new Hd44780LcdConnection(settings, pins);
        }

        //Clears the display
        public override void Clear()
        {
            _lcd.Clear();
            _cursorPosition = new Hd44780Position();
        }

        //Writes value to the display
        public override void Write(object value)
        {
            _lcd.Write(value);
            _cursorPosition.Column++;
        }
        
        //Writes value + newline to te display
        public override void WriteLine(object value)
        {
            _lcd.WriteLine(value);
            _cursorPosition.Column = 0;
            _cursorPosition.Row++;
        }

        //Setup Hd44780 connection when connected directly to the Raspberry Pi
        public static Hd44780Pins LoadGpioConfiguration(ProcessorPin registerSelectPin, ProcessorPin clockPin, IEnumerable<ProcessorPin> dataPins)
        {
            IGpioConnectionDriver driver = GpioConnectionSettings.DefaultDriver;
            return new Hd44780Pins(
                driver.Out(registerSelectPin),
                driver.Out(clockPin),
                dataPins.Select(p => (IOutputBinaryPin)driver.Out(p)));
        }

        //Setup Hd44780 connection when connected to the Raspberry Pi though a Pcf8574 I2c io expander
        public static Hd44780Pins SetupPcf8574Connection(int I2CAddress)
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
            Pcf8574I2CConnection connection = new Pcf8574I2CConnection(driver.Connect(I2CAddress));
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