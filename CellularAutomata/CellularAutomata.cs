namespace CellularAutomata
{
    public abstract class CellularAutomata
    {
        protected BitMatrix State { get; set; }

        public abstract void Iterate();

        public abstract void Draw();

        public abstract void SetupConsole();
    }
}