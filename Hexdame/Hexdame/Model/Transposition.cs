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
        public Move BestMove;

        public Transposition(int lowerbound, int upperbound, int depth, Move bestMove)
        {
            this.Lowerbound = lowerbound;
            this.Upperbound = upperbound;
            this.Depth = depth;
            this.BestMove = bestMove;
        }
    }
}
