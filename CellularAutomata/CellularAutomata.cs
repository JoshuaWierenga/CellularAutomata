namespace CellularAutomata
{
    public abstract class CellularAutomata
    {
        protected uint TimesRan { get; set; }

        protected int Delay { get; set; }

        protected BitMatrix State { get; set; }

        public abstract void Iterate();

        public abstract void Draw();

        public abstract void SetupConsole();

        public abstract void Modify(string[] arguments);
    }
}