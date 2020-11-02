using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroPlayerGame
{
    public class Cell
    {
        public bool IsAlive { get; private set; } = false;
        public int x { get; private set; }
        public int y { get; private set; }
        public Cell(int x, int y, bool status)
        {
            IsAlive = status;
            this.x = x;
            this.y = y;
        }
    }
}
