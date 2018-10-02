namespace CellularAutomata.Automata
{
    public enum Modification
    {
        //Colour changing
        Colour,
        //Draw delay
        Delay,
        //Draw direction, use for reversible CAs
        Direction,
        //Allows stopping and starting CAs
        Running
    }
}