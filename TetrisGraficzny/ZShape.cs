using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGraficzny
{
    class ZShape:IBlock
    {

        public int Width { get; set; }
        public bool[,] Arr { get; set; }
        public short Color { get; set; }
        public ZShape()
        {
            Width = 3;
            Arr = new bool[3, 3];
            Arr[0, 1] = true;
            Arr[1, 1] = true;
            Arr[1, 2] = true;
            Arr[2, 2] = true;
            Color = 1;
        }

        public void ChPos(bool dir)
        {
            if (dir)
            {
                for (int i = Width - 1; i > 0; --i)
                    for (int j = 0; j < Width; ++j)
                        Arr[i, j] = Arr[i - 1, j];
                Arr[0, 0] = false;
                Arr[0, 1] = false;
                Arr[0, 2] = false;
            }
            else
            {
                for (int i = 0; i < Width - 1; ++i)
                    for (int j = 0; j < Width; ++j)
                        Arr[i, j] = Arr[i + 1, j];
                Arr[2, 0] = false;
                Arr[2, 1] = false;
                Arr[2, 2] = false;
            }
        }
        public void Rotate()
        {
            bool[,] temp = new bool[3, 3];
            for (int i = 0; i < 3; ++i)
                for (int j = 0; j < 3; ++j)
                    temp[j, 2 - i] = Arr[i, j];
            Arr = temp;
        }
    }
}
