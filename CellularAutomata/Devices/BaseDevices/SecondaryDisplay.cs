using System.Drawing;

namespace CellularAutomata.Devices.BaseDevices
{
    public abstract class SecondaryDisplay
    {    
        public abstract Size DisplaySize { get; protected set; }

        //Gets or sets cursor vertical postion
        public abstract int CursorTop { get; set; }

        //Gets or sets cursor horizontal postion
        public abstract int CursorLeft { get; set; }

        public abstract bool CursorVisible { get; set; }

        //Clears the display
        public abstract void Clear();

        //Writes value to the display
        public abstract void Write(object value);

        //Writes value + newline to te display
        public abstract void WriteLine(object value);
    }
}