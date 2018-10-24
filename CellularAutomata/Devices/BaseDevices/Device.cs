namespace CellularAutomata.Devices.BaseDevices
{
    public abstract class Device
    {
        //Handles program input    
        public Input Input { get; protected set; }

        //Handles any connected secondary displays e.g. second console, small lcd or oled screens
        public SecondaryDisplay SecondaryDisplay { get; protected set; }

        //Shows request and options to the user and returns selected option
        //location controls which screen the request and/or options are displayed on
        //clear controls whether or not to clear the location display before displaying request and/or options
        public abstract string GetOption(OutputLocation location, string request, string[] options, bool clear);

        //Asks user for a number between 0 and maxNumber and returns it
        //clear controls whether or not to clear the display before displaying request
        public abstract int GetNumber(string request, int maxNumber, bool clear);
    }
}