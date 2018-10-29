using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CellularAutomata.Automata;
using CellularAutomata.Devices;
using CellularAutomata.Devices.BaseDevices;

namespace CellularAutomata
{
    internal static class Program
    {
        private static readonly Dictionary<string, Type> Automata = new Dictionary<string, Type>
        {
            {"Elementary Automata", typeof(ElementaryCa)},
            {"Second Order Reversible Automata", typeof(SecondOrderReversibleCa)},
            {"Three Colour Totalistic Automata", typeof(ThreeColourTotalisticCa)}
        };

        private static void Main(string[] args)
        {
            //Device device = new PiLcdAndKeypad(PiHd44780Lcd.SetupPcf8574Connection(0x27));
            Device device = new ConsoleAndKeyboard();

            Automata.CellularAutomata ca = SelectAutomata(device);

            ca.SetupConsole();

            while (true)
            {
                //Allow automata modification asynchronously to avoid blocking ca iteration
                //while actual modification must happen with iteration is not occuring, modification setup can
                //TODO fix positioning on console when not using a secondary screen
                //Task<string> input = Task.Run(() => device.GetOption(OutputLocation.Secondary, "Modify Automata", ca.Modifications.Keys.ToArray(), false));
                //Task<ConsoleKeyInfo> input = Task.Run(() => device.Input.ReadKey(true, InputType.Numbers));
                //while (!input.IsCompleted)
                //{
                ca.Draw();
                ca.Iterate();
                //}

                //if (input.Result.Key != ConsoleKey.Enter) continue;

                //Modification automata = Modification.CreateModification(ca.Modifications[option].modification, device);
                //automata.ApplyModification(ca);
            }
        }

        private static Automata.CellularAutomata SelectAutomata(Device device)
        {
            //Ask user to pick automata
            //TODO mention this is the time to set size of console window
            string option = device.GetOption(OutputLocation.Both, "Select Automata", Automata.Keys.ToArray(), true);

            //Create instance of selected automata
            return (Automata.CellularAutomata)Activator.CreateInstance(Automata[option], device);
        }
    }
}