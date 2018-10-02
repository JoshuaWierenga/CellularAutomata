namespace CellularAutomata
{
    public static class Int2DExtensions
    {
        public static void ShiftBack(this int[,] array)
        {
            for (int i = 1; i <= array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    if (i == array.GetLength(0))
                    {
                        array[i - 1, j] = 0;
                    }
                    else
                    {
                        array[i - 1, j] = array[i, j];
                    }
                }
            }
        }
    }
}