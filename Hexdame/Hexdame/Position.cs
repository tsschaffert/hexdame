using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame
{
    struct Position
    {
        private int number;
        private int character;

        public Position(int number, char character)
            : this(number, (int)(character - 'a'))
        {
            
        }

        public Position(int number, int character)
        {
            this.number = number;
            this.character = character;
        }

        public int GetArrayIndexX()
        {
            return number;
        }

        public int GetArrayIndexY()
        {
            int x = number;
            int y = character;
            if (x > 4)
            {
                y -= x - 4;
            }
            return y;
        }
    }
}
