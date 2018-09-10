//From https://www.pvladov.com/2012/05/bit-matrix-in-c-sharp.html

using System;

namespace CellularAutomata
{
    //Stores a 2d array of bits
    public class BitMatrix
    {
        public uint RowCount { get; }
        public uint ColumnCount { get; }
        private byte[] Data { get; }

        //Only accepts uints as matrix can only have positive lengths
        public BitMatrix(uint rowCount, uint columnCount)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;

            // Calculate the needed number of bits and bytes
            uint bitCount = RowCount * ColumnCount;
            uint byteCount = bitCount >> 3;
            if (bitCount % 8 != 0)
            {
                byteCount++;
            }

            // Allocate the needed number of bytes
            Data = new byte[byteCount];
        }

        //Gets/Sets the value at the specified row and column index.
        public bool this[uint rowIndex, uint columnIndex]
        {
            get
            {
                //Create exception if requested position is not in matrix
                if (rowIndex >= RowCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                if (columnIndex >= ColumnCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(columnIndex));
                }

                //Converts 2d position to 1d position
                // (x, y) = (x * xLength + y)
                uint pos = rowIndex * ColumnCount + columnIndex;

                //Finds offset from last byte
                int offset = (int)pos % 8;
                //Divides position by 8 to get which byte the bit is in
                pos >>= 3;

                //Comparing the byte at pos to 0
                return (Data[pos] & 1 << offset) != 0;
            }

            set
            {
                //Create exception if requested position is not in matrix
                if (rowIndex >= RowCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                }
                if (columnIndex >= ColumnCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(columnIndex));
                }

                //Converts 2d position to 1d position
                // (x, y) = (x * xLength + y)
                uint pos = rowIndex * ColumnCount + columnIndex;

                //Finds offset from end of byte
                int offset = (int)pos % 8;
                //Divides position by 8 to get which byte the bit is in
                pos >>= 3;

                //Switches bit at offset off if it is swiched on without modifiying other bits
                Data[pos] &= (byte)~(1 << offset);

                if (value)
                {
                    //Switches bit at offset on without modifying other bits
                    Data[pos] |= (byte)(1 << offset);
                }
            }
        }

        //Shifts each row back to clear last row
        //this will remove the first row
        public void ShiftBack()
        {
            for (uint i = 0; i < RowCount; i++)
            {
                for (uint j = 0; j < ColumnCount; j++)
                {
                    //if row is not last then set bit to bit in next row
                    //if row is last row then set it to false
                    this[i, j] = i < RowCount - 1 && this[i + 1, j];
                }
            }
        }
    }
}