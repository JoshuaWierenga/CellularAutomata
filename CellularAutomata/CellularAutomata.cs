namespace CellularAutomata
{
    public abstract class CellularAutomata
    {
        public uint CurrentRow { get; protected set; }

        public BitMatrix State { get; protected set; }

        public abstract void Iterate();

        public abstract void Draw();
    }
}