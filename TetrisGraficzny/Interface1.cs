using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGraficzny
{
    public interface IBlock
    {
        int Width { get; set; }
        bool[,] Arr { get; set; }
        short Color { get; set; }
        void ChPos(bool dir);
        void Rotate();
    }
}
