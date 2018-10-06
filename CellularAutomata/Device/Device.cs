using System;

namespace CellularAutomata.Device
{
    public abstract class Device
    {
        public abstract int CursorTop { get; set; }

        public abstract int CursorLeft { get; set; }

        public abstract bool CursorVisible { get; set; }

        public abstract void Clear();

        public abstract ConsoleKeyInfo ReadKey(bool intercept);

        public abstract string ReadLine();

        public abstract void WriteLine(object value);

        public abstract void Write(object value);

        public abstract string GetOption(string request, string[] options, bool resetClear);

        public abstract int GetNumber(string request, int maxNumber, bool resetClear);
    }
}