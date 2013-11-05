using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame
{
    class Move
    {
        private List<Position> positions;

        public Move(params Position[] positions)
        {
            this.positions = new List<Position>();
            for (int i = 0; i < positions.Length; i++)
            {
                this.positions.Add(positions[i]);
            }
        }

        public Position GetStartingPosition()
        {
            return GetPosition(0);
        }

        public Position GetPosition(int index)
        {
            return positions[index];
        }

        public int GetNumberOfPositions()
        {
            return positions.Count;
        }

        public static bool operator ==(Move a, Move b)
        {
            for (int i = 0; i < a.positions.Count; i++)
            {
                if (a.positions[i] != b.positions[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator !=(Move a, Move b)
        {
            return !(a == b);
        }
    }
}
