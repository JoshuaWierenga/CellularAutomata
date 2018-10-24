using System;
using CellularAutomata.Devices.BaseDevices;

namespace CellularAutomata.Devices.Inputs
{
    class Keyboard : Input
    {
        public sealed override bool ReducedInput { get; protected set; }

        public Keyboard()
        {
            //Keyboards have both numbers and arrows so special setup is not needed to handle both
            ReducedInput = false;
        }

        //Handles getting a single character input from user
        //intercept controls whether to stop input being written the screen, input controls whether to
        //get number or arrow entry, it is ignored however for keyboards since both are possible at once
        //TODO limit input
        public override ConsoleKeyInfo ReadKey(bool intercept, InputType input)
        {
            //Can ignore inputType since keyboard contains both numbers and arrows
            return Console.ReadKey(intercept);
        }

        //Handles getting an entire string from the user
        //TODO limit input
        public override string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}