using System;
using System.Collections.Generic;
using System.Linq;
using CellularAutomata.Automata;

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
            Automata.CellularAutomata ca = SelectAutomata();

            ca.SetupConsole();

            while (true)
            {
                ca.Draw();
                ca.Iterate();
            }
        }

        private static Automata.CellularAutomata SelectAutomata()
        {
            string option = UserRequest.GetOption("Select Automata", Automata.Keys.ToArray(), true);

            return (Automata.CellularAutomata)Activator.CreateInstance(Automata[option]);
        }
    }
}