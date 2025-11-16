using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR3_3D.Models
{
    public class DepthMap
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public double[,] Data { get; set; }

        public bool IsOutOfBounds(int x, int y) => x < 0 || x >= Width || y < 0 || y >= Height;
        public bool IsValidDepth(int x, int y) => !IsOutOfBounds(x, y) && Math.Abs(Data[y, x]) > 0.0001;
    }
}
