﻿using System;
using System.Collections.Generic;
using System.Linq;
using CellularAutomata.Automata;
using CellularAutomata.Device;

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
            //Device.Device device = new PiLcdAndKeypad(0x27, PiLcdAndKeypad.SetupPcf8574Connection(0x27));
            Device.Device device = new ConsoleAndKeyboard();

            Automata.CellularAutomata ca = SelectAutomata(device);

            ca.SetupConsole();

            while (true)
            {
                ca.Draw();
                ca.Iterate();
            }
        }

        private static Automata.CellularAutomata SelectAutomata(Device.Device device)
        {
            //Ask user to pick automata
            //TODO mention this is the time to set size of console window
            string option = device.GetOption("Select Automata", Automata.Keys.ToArray(), true);

            //Create instance of selected automata
            return (Automata.CellularAutomata)Activator.CreateInstance(Automata[option], device);
        }
    }
}