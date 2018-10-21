using System.Drawing;

namespace CellularAutomata.Devices.BaseDevices
{
    public abstract class SecondaryDisplay
    {    
        public abstract Size DisplaySize { get; protected set; }

        public abstract int CursorTop { get; set; }

        public abstract int CursorLeft { get; set; }

        public abstract bool CursorVisible { get; set; }

        public abstract void Clear();

        public abstract void Write(object value);

        public abstract void WriteLine(object value);
    }
}