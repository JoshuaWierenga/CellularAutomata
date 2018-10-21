using System;
using CellularAutomata.Devices.BaseDevices;

namespace CellularAutomata.Devices.Inputs
{
    class Keyboard : Input
    {
        public sealed override bool ReducedInput { get; protected set; }

        public Keyboard()
        {
            ReducedInput = false;
        }

        //Reads a char form some form of keyboard, in this case the console
        //intercept allows preventing the key from appearing on screen
        public override ConsoleKeyInfo ReadKey(bool intercept, InputType input)
        {
            //Can ignore inputType since keyboard contains both numbers and arrows
            return Console.ReadKey(intercept);
        }

        //Reads a string form some form of keyboard, in this case the console
        public override string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}
