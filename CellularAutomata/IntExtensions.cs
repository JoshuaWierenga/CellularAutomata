namespace CellularAutomata
{
    public static class IntExtensions
    {
        //Converts integers from base 10 to any other base up to base 9
        //From https://stackoverflow.com/a/923814
        public static string BaseChange(int number, int newBase)
        {
            if (newBase >= 10) return "";

            //Buffer to store the new number while converting
            int i = 32;
            char[] buffer = new char[i];

            //Divides the number by the largest power of newBase that fits in the number and stores the remainder in the buffer
            //this is repeated while dividing the number by newBase until the number is zero
            do
            {
                buffer[--i] = (char)(number % newBase + 48);
                number = number / newBase;
            }
            while (number > 0);

            //Returns only the part that stores the new number
            return new string(buffer, i, 32 - i);
        }

        //Removes the first row of the array and moves all other rows back by one row
        public static void ShiftBack(this int[,] array)
        {
            for (int i = 1; i <= array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    //Clear last row as there is no row to replace it with
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