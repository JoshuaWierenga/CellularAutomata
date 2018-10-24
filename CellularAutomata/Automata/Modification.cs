namespace CellularAutomata.Automata
{
    public enum Modification
    {
        //Colour changing
        Colour,
        //Draw delay
        Delay,
        //Draw direction, use for reversible CAs
        //TODO move to reversible CA as it does nothing for most CAs
        Direction,
        //Allows stopping and starting CAs
        Running
    }
}