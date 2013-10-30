﻿using System;
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
            : this(number, (int)(character - 'a' + 1))
        {
            
        }

        public Position(int number, int character)
        {
            this.number = number;
            this.character = character;
        }

        public int GetArrayIndexX()
        {
            return number - 1;
        }

        public int GetArrayIndexY()
        {
            int x = GetArrayIndexX();
            int y = character - 1;
            if (x > 4)
            {
                y -= x - 4;
            }
            return y;
        }

        public static bool operator ==(Position a, Position b)
        {
            return a.character == b.character && a.number == b.number;
        }

        public static bool operator !=(Position a, Position b)
        {
            return !(a==b);
        }
    }
}
