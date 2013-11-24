using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexdame.Model
{
    class Transposition
    {
        public int Upperbound;
        public int Lowerbound;
        public int Depth;

        public Transposition(int lowerbound, int upperbound, int depth)
        {
            this.Lowerbound = lowerbound;
            this.Upperbound = upperbound;
            this.Depth = depth;
        }
    }
}
