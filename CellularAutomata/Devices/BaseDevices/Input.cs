using System;

namespace CellularAutomata.Devices.BaseDevices
{
    public abstract class Input
    {
        //Whether arrows and numbers are enterable at once or not
        public abstract bool ReducedInput { get; protected set; }

        //Handles getting a single character input from user, this could come from a keyboard or keypad
        //or any other kind of input capable of both number and arrow entry, not necessarily at the same time
        //intercept controls whether to stop input being written the screen, input controls whether to get
        //number or arrow entry, this is ignored on input devices capable of both at once
        public abstract ConsoleKeyInfo ReadKey(bool intercept, InputType input);

        //Handles getting an entire string from the user, this could come from a keyboard or keypad
        //or any other kind of input capable of number input
        public abstract string ReadLine();
    }
}