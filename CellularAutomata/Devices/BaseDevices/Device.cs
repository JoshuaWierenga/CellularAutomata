namespace CellularAutomata.Devices.BaseDevices
{
    public abstract class Device
    {
        public Input Input { get; protected set; }

        public SecondaryDisplay SecondaryDisplay { get; protected set; }

        public abstract string GetOption(string request, string[] options, bool resetClear);

        public abstract int GetNumber(string request, int maxNumber, bool resetClear);
    }
}