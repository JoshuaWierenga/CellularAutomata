using System;
using System.Collections.Generic;

namespace CellularAutomata.Automata
{
    public class AutomataDefaults
    {
        //Default colours used by CAs, most are binary CAs so 2 colours is fine
        public static readonly ConsoleColor[] DefaultColours = {
            //Used for cells with a value of 0
            ConsoleColor.White,
            //Used for cells with a value of 1
            ConsoleColor.Black
        };

        public static readonly Dictionary<string, int> DefaultDelays = new Dictionary<string, int>
        {
            {"Very Fast (25ms)", 25},
            {"Fast (50ms)", 50},
            {"Medium (100ms)", 100},
            {"Slow (150ms)", 150},
            {"Very Slow (200ms)", 200}
        };
    }
}