﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame
{
    /// <summary>
    /// A position consists of a number and a character to specify a place on the game board. The number starts with 1, the character with a. A number can be used for the character as well.
    /// </summary>
    public struct Position
    {
        private int number;
        private int character;

        public int Number
        {
            get { return number; }
        }

        public int Character
        {
            get { return character; }
        }

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

        public override bool Equals(object obj)
        {
            if (obj is Position)
            {
                Position pos = (Position)obj;
                return pos == this;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Number * 10 + Character;
        }

        public override string ToString()
        {
            return "("+Number+","+Character+")";
        }
    }
}
