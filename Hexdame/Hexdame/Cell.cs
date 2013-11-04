using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame
{
    public class Cell : ICloneable
    {
        public enum Occupancy { Empty = 0, White = 2, WhiteKing = 3, Red = 4, RedKing = 5 };

        private Occupancy content;

        public Occupancy Content 
        { 
            set { content = value; } 
            get { return content; }
        }

        public bool ContainsKing
        {
            get { return ((int)content & 1) == 1; }
        }

        public bool ContainsWhite
        {
            get { return ((int)content & 2) == 1; }
        }

        public bool ContainsRed
        {
            get { return ((int)content & 4) == 1; }
        }

        public Cell(Occupancy content)
        {
            this.content = content;
        }

        public override string ToString()
        {
            return Content.ToString("d");
        }

        public object Clone()
        {
            return new Cell(this.Content);
        }
    }
}
