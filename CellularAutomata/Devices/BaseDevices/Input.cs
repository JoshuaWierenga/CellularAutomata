using System;

namespace CellularAutomata.Devices.BaseDevices
{
    public abstract class Input
    {
        //Whether arrows and numbers are enterable at once or not
        public abstract bool ReducedInput { get; protected set; }

        public abstract ConsoleKeyInfo ReadKey(bool intercept, InputType input);

        public abstract string ReadLine();
    }
}