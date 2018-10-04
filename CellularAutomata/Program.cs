using CellularAutomata.Automata;

namespace CellularAutomata
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            //Automata.CellularAutomata ca = new SecondOrderReversibleCa();

            //Automata.CellularAutomata ca = new ElementaryCa();

            Automata.CellularAutomata ca = new ThreeColourTotalisticCa();

            ca.SetupConsole();

            for (int i = 0; i < 500; i++)
            {
                for (int j = 0; j < 23; j++)
                {
                    ca.Draw();
                    ca.Iterate();
                }

                //ca.Modify(Modification.Direction);
            }
        }
    }
}