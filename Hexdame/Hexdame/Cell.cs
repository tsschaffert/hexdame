using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame
{
    public enum Occupancy { Empty = 0, White = 2, WhiteKing = 3, Red = 4, RedKing = 5 };

    class Cell
    {
        private Occupancy content;

        public Occupancy Content 
        { 
            set { content = value; } 
            get { return content; }
        }

        public bool IsKing
        {
            get { return ((int)content & 1) == 1; }
        }

        public Cell(Occupancy content)
        {
            this.content = content;
        }

        public override string ToString()
        {
            return Content.ToString("d");
        }
    }
}
